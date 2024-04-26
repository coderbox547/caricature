using Newtonsoft.Json;

namespace Data.Contract
{
    public class ApiResponse
    {
        [JsonProperty("data")]
        public ApiData Data { get; set; }


        [JsonProperty("error_detail")]
        public ErrorDetail ErrorDetail { get; set; }

    }

    public class ApiData
    {
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }

    public class ErrorDetail
    {
        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("code_message")]
        public string CodeMessage { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
