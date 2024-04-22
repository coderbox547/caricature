
using Api.Helpers;
using Data.Domain;
using Data.Extensions;
using Data.Repository.Services;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Api.Helpers.IdentityException;

namespace Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void ConfigureCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<LoggingMiddleware>();
        }
    }

    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private ILogExceptionService _logExceptionService;


        public LoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            //_logger = logger;
            _next = next;
            //_logExceptionService = logExceptionService;
            _logger = loggerFactory.CreateLogger(typeof(LoggingMiddleware));

        }

        public async Task InvokeAsync(HttpContext httpContext, ILogExceptionService logExceptionService, ILogActivityService logActivityService)
        {
            _logExceptionService = logExceptionService;

            LogActivity logActivity = new LogActivity();
            LogException _logException = new LogException();

            DateTime _requestSendTime = DateTime.UtcNow.GetIndianCurrentDate();

            await LogRequest(httpContext, logActivity);

            var originalResponseBody = httpContext.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                try
                {
                    httpContext.Response.Body = responseBody;
                    await _next.Invoke(httpContext);

                    logActivity.Response = await LogResponse(httpContext, responseBody, originalResponseBody);
                }
                catch (IdentityException ex)
                {
                    await ReturnExceptionAsResponseAsync(httpContext, new Exception(string.Join(",", ex.Errors)), HttpStatusCode.InternalServerError);
                }
                catch (Unauthorized ex)
                {
                    await ReturnExceptionAsResponseAsync(httpContext, ex, HttpStatusCode.Unauthorized, string.Concat("challenge-token-expired|", ex.InformativeMsg));
                }
                catch (InformativeException ex)
                {
                    await ReturnExceptionAsResponseAsync(httpContext, ex, HttpStatusCode.NotFound, ex.InformativeMsg);
                }
                catch (Exception ex)
                {
                    _logException = await SaveExceptionIntoDB(ex);
                    await ReturnExceptionAsResponseAsync(httpContext, ex, HttpStatusCode.InternalServerError);
                }
                finally
                {

                    if (!string.IsNullOrEmpty(logActivity.Endpoint) && !logActivity.Endpoint.ToLower().Contains("swagger"))
                    {
                        // save the log activities
                        if (_logException.Id > 0)
                        {
                            logActivity.Response = _logException.Message;
                            logActivity.LogExceptionId =  _logException.Id;
                        }

                        logActivity.RequestSentTime = _requestSendTime.ConvertTimeAsString();
                        logActivity.RequestCompleteTime = DateTime.UtcNow.GetIndianCurrentDate().ConvertTimeAsString();
                        logActivity.Duration = DateTime.UtcNow.GetIndianCurrentDate().GetTimeBetweenDates(_requestSendTime);
                        await logActivityService.SaveAsync(logActivity);
                    }
                }
            }
        }

        private Task ReturnExceptionAsResponseAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, string message = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            Exception _exception = null;

            if (statusCode != HttpStatusCode.Unauthorized && statusCode != HttpStatusCode.NotFound)
            {
                _exception = GetInnerException(exception);
            }


            if (message == null)
                message = _exception.Message;


            ErrorDetail error = new ErrorDetail
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
                //Exception = statusCode == HttpStatusCode.Unauthorized ? null : _exception.Message
                Exception = _exception == null ? null : _exception.Message
            };

            var stringAsJson = JsonConvert.SerializeObject(error);

            return context.Response.WriteAsync(stringAsJson);
        }

        private async Task<LogException> SaveExceptionIntoDB(Exception ex)
        {
            _logger.LogInformation($"Something went wrong: {ex}");

            Exception _innerException = GetInnerException(ex);

            string stackTrace = ex.StackTrace.ToString();

            LogException _logException = new LogException
            {
                ExceptionType = ExceptionType.General,
                StackTrace = stackTrace.Length > 4000 ? stackTrace.Substring(0, 3999) : ex.StackTrace.ToString(),
                Message = _innerException.Message.ToString(),
            };


            await _logExceptionService.CreateAsync(_logException);

            return _logException;
        }

        private Exception GetInnerException(Exception ex)
        {
            Exception _exception = ex;

            while (_exception.InnerException != null)
            {
                _exception = _exception.InnerException;
            }

            return _exception;
        }


        private async Task<string> LogResponse(HttpContext context, MemoryStream responseBody, Stream originalResponseBody)
        {
            var responseContent = new StringBuilder();
            //responseContent.AppendLine("=== Response Info ===");

            //responseContent.AppendLine("-- headers");
            //foreach (var (headerKey, headerValue) in context.Response.Headers)
            //{
            //    responseContent.AppendLine($"header = {headerKey}    value = {headerValue}");
            //}

            //responseContent.AppendLine("-- body");
            responseBody.Position = 0;
            var content = await new StreamReader(responseBody).ReadToEndAsync();
            responseContent.AppendLine(content);
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;

            //_logger.LogInformation(responseContent.ToString());

            return responseContent.ToString();
        }

        private async Task LogRequest(HttpContext context, LogActivity logActivity)
        {
            var requestContent = new StringBuilder();
            var headers = new StringBuilder();

            requestContent.AppendLine("=== Request Info ===");

            requestContent.AppendLine("-- headers");
            foreach (var (headerKey, headerValue) in context.Request.Headers)
            {
                headers.AppendLine($"header = {headerKey}    value = {headerValue}");
            }

            requestContent.AppendLine("-- body");
            context.Request.EnableBuffering();
            var requestReader = new StreamReader(context.Request.Body);
            var content = await requestReader.ReadToEndAsync();
            requestContent.AppendLine($"{content}");

            _logger.LogInformation(requestContent.ToString());
            context.Request.Body.Position = 0;


            logActivity.Method = context.Request.Method.ToUpper();
            logActivity.Endpoint = context.Request.Path + context.Request.QueryString;
            logActivity.Request = requestContent.ToString();
            logActivity.Headers = headers.ToString();
        }
    }


    public class ErrorDetail
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Exception { get; set; }
    }
}
