using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Contract.Response
{
    public class GetResponse<T> : ErrorHandling
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }


    public class SearchResponse<T> : ErrorHandling
    {
        public SearchResponse()
        {
            Data = new List<T>();
        }

        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
    }

    public class SaveOrUpdateOrDeleteResponse<T> : ErrorHandling
    {
        public bool Success { get; set; }

        public T Data { get; set; }
        public string Message { get; set; }
    }

    public class SaveOrUpdateOrDeleteResponse : ErrorHandling
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        
    }

    public abstract class ErrorHandling
    {
        public ErrorHandling()
        {
            Errors = new List<ErrorDescription>();
        }

        [JsonIgnore]
        public List<ErrorDescription> Errors { get; set; }

        [JsonIgnore]
        public bool IsErrorOccured => Errors.Any();

        [JsonIgnore]
        public string GetErrorsWithCommaSeperate => string.Join(',', Errors.Select(x => x.Message).ToArray());

        public void LogError(string message, ErrorType errorType, Exception exception = null)
        {
            Errors.Add(new ErrorDescription(message, errorType, exception));
        }
        public void LogDebug(string message, ErrorType errorType, Exception exception = null)
        {
            Errors.Add(new ErrorDescription(message, errorType, exception));
        }
        public class ErrorDescription
        {
            public ErrorDescription(string message, ErrorType errorType, Exception exception)
            {
                Message = message;
                ErrorType = errorType;
                Exception = exception;
            }
            public string Message { get; set; }
            public ErrorType ErrorType { get; set; }

            [JsonIgnore]
            public Exception Exception { get; set; }
        }
    }
}
