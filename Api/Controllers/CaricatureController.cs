using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace CaricatureAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaricatureController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey = "Pw8598NOLBLeFugcZtXCWn1lPCcRV4374aj0WhDEqem6Vb69jpBI1hHD3S2fUxUQ";

        public CaricatureController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();

        }

        [HttpPost("upload/{type}")]
        public async Task<IActionResult> UploadPhoto(IFormFile photo, string type)
        {
            try
            {
                var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                var folderName = Guid.NewGuid().ToString();
                var folderPath = Path.Combine(uploadsFolderPath, folderName);
                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, photo.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                var apiUrl = "https://www.ailabapi.com/api/portrait/effects/portrait-animation";
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Headers.Add("ailabapi-api-key", apiKey);

                using (var content = new MultipartFormDataContent())
                {

                    content.Add(new StreamContent(photo.OpenReadStream()), "image", photo.FileName);
                    content.Add(new StringContent(type), "type");

                    request.Content = content;

                    var response = await _httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    string responseContent = await response.Content.ReadAsStringAsync();
                    ApiResponse responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                    string imageUrl = responseObject.Data.ImageUrl;

                    var responseImageFilePath = Path.Combine(folderPath, $"{folderName}_{type}_response_image.jpg");

                    using (var imageResponse = await _httpClient.GetAsync(imageUrl))
                    using (var fileStream = new FileStream(responseImageFilePath, FileMode.Create))
                    {
                        await imageResponse.Content.CopyToAsync(fileStream);
                    }



                    return Ok(new { ResponseImageFilePath = responseImageFilePath, Imagename = folderName });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpPost("RemoveBackground/{folderName}")]
        public async Task<IActionResult> RemoveBackground(IFormFile image, string folderName)
        {

            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");



            var folderPath = Path.Combine(uploadsFolderPath, folderName);
            Directory.CreateDirectory(folderPath);


            string apiUrl = "https://www.ailabapi.com/api/cutout/portrait/portrait-background-removal";


            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Headers.Add("ailabapi-api-key", apiKey);

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(image.OpenReadStream()), "image", image.FileName);
                content.Add(new StringContent("whiteBK"), "return_form");

                request.Content = content;

                var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                ApiResponse responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                string imageUrl = responseObject.Data.ImageUrl;

                var responseImageFilePath = Path.Combine(folderPath, $"{folderName}__response_image.jpg");

                using (var imageResponse = await _httpClient.GetAsync(imageUrl))
                using (var fileStream = new FileStream(responseImageFilePath, FileMode.Create))
                {
                    await imageResponse.Content.CopyToAsync(fileStream);
                }
                return Ok(imageUrl);
            }
        }
   


        public class ApiResponse
        {
            [JsonProperty("data")]
            public ApiData Data { get; set; }

            [JsonProperty("error_code")]
            public int ErrorCode { get; set; }

            [JsonProperty("error_detail")]
            public ErrorDetail ErrorDetail { get; set; }

            [JsonProperty("log_id")]
            public string LogId { get; set; }

            [JsonProperty("request_id")]
            public string RequestId { get; set; }
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








}




