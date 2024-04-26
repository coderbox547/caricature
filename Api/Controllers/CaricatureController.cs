using Api.Controllers;

using Data.Contract;
using Data.Contract.Request;
using Data.Contract.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace CaricatureAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaricatureController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey = "Pw8598NOLBLeFugcZtXCWn1lPCcRV4374aj0WhDEqem6Vb69jpBI1hHD3S2fUxUQ";

        public CaricatureController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();

        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto(IFormFile image)
        {


            if (image == null || image.Length == 0)
            {
                throw new Exception("No image provided.");
            }
            var fileExtension = Path.GetExtension(image.FileName).ToLower();

            if (!IsAllowedExtension(fileExtension))
            {
                throw new Exception("Only JPG and PNG files are allowed.");
            }
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var folderName = Guid.NewGuid().ToString();
            var folderPath = Path.Combine(uploadsFolderPath, folderName);
            Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, "originalImage.jpg");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Success(new { folderName = folderName });
        }

        [HttpPost("GenerateCaricature/{folderName}/{type}")]
        public async Task<IActionResult> GenerateCaricature(string folderName, string type)
        {
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderName);
            var existingfileType = Path.Combine(uploadsFolderPath, $"{type}.jpg");
            var caricratureResponse = new CaricatureResponse();

            if (!Directory.Exists(uploadsFolderPath))
            {
                throw new Exception("Folder not found");
            }
            if (System.IO.File.Exists(existingfileType))
            {
                var relativePath = existingfileType.Replace(Directory.GetCurrentDirectory(), "").Replace("\\", "/").TrimStart('/');
                var imageUrl = $"{Request.Scheme}://{Request.Host}/{relativePath}";
                caricratureResponse.ImageUrl = imageUrl;
                return Success(caricratureResponse);
            }

            var orginalFilePath = Path.Combine(uploadsFolderPath, "originalImage.jpg");

            if (!System.IO.File.Exists(orginalFilePath))
            {
                throw new Exception("File not found.");
            }

            var apiUrl = "https://www.ailabapi.com/api/portrait/effects/portrait-animation";
            var request_caracrature = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request_caracrature.Headers.Add("ailabapi-api-key", apiKey);

            using (var content_caracrature = new MultipartFormDataContent())
            {

                content_caracrature.Add(new StreamContent(System.IO.File.OpenRead(orginalFilePath)), "image", "originalImage.jpg");
                content_caracrature.Add(new StringContent(type), "type");

                request_caracrature.Content = content_caracrature;

                var response_caracrature = await _httpClient.SendAsync(request_caracrature);
                response_caracrature.EnsureSuccessStatusCode();

                string responseContent = await response_caracrature.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                if (responseObject != null && responseObject.Data != null && !string.IsNullOrEmpty(responseObject.Data.ImageUrl))
                {
                    string imageUrl = responseObject.Data.ImageUrl;
                    var caricatureImageFilePath = Path.Combine(uploadsFolderPath, $"{type}.jpg");

                    using (var imageResponse = await _httpClient.GetAsync(imageUrl))
                    using (var fileStream = new FileStream(caricatureImageFilePath, FileMode.Create))
                    {
                        await imageResponse.Content.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                    }
                    caricratureResponse.ImageUrl = imageUrl;

                    return Success(caricratureResponse);
                }
                else
                {
                    throw new Exception("Error in Image processing.");
                }
            }
        }

        [HttpPost("GenerateImage")]
        public async Task<IActionResult> GenerateImage(GenerateImageRequest model)
        {

            var apiUrl_bgremoval = "https://www.ailabapi.com/api/cutout/portrait/portrait-background-removal";
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl_bgremoval);
            request.Headers.Add("ailabapi-api-key", apiKey);

            var content = new MultipartFormDataContent();

            using (var imageStream = await _httpClient.GetStreamAsync(model.ImageUrl))
            {
                content.Add(new StreamContent(imageStream), "image", "response_image.jpg");
                content.Add(new StringContent("whiteBK"), "return_form");

                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseContentRemoveBg = await response.Content.ReadAsStringAsync();
                var objectResponse = JsonConvert.DeserializeObject<ApiResponse>(responseContentRemoveBg);

                if (objectResponse != null && objectResponse.Data != null && !string.IsNullOrEmpty(objectResponse.Data.ImageUrl))
                {
                    var finalreponse = new CaricatureResponse
                    {
                        ImageUrl = objectResponse.Data.ImageUrl

                    };

                    return Success(finalreponse);
                }
               

            }
            return BadRequest("Failed to remove background from the image");


        }


        private bool IsAllowedExtension(string fileExtension)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            return allowedExtensions.Contains(fileExtension);
        }


    }
}















