
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

        public LoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            //_logger = logger;
            _next = next;
            //_logExceptionService = logExceptionService;
            _logger = loggerFactory.CreateLogger(typeof(LoggingMiddleware));

        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (IdentityException ex)
            {
                await ReturnExceptionAsResponseAsync(httpContext, new Exception(string.Join(",", ex.Errors)), HttpStatusCode.InternalServerError);
            }
            catch (InformativeException ex)
            {
                await ReturnExceptionAsResponseAsync(httpContext, ex, HttpStatusCode.NotFound, ex.InformativeMsg);
            }
            catch (Exception ex)
            {
                await ReturnExceptionAsResponseAsync(httpContext, ex, HttpStatusCode.InternalServerError);
            }
            finally
            {
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

        private Exception GetInnerException(Exception ex)
        {
            Exception _exception = ex;

            while (_exception.InnerException != null)
            {
                _exception = _exception.InnerException;
            }

            return _exception;
        }

    }


    public class ErrorDetail
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Exception { get; set; }
    }
}
