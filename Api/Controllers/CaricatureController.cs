using Api.Models;
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

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto(IFormFile image)
        {


            if (image == null || image.Length == 0)
            {
                return BadRequest("No image provided.");
            }
            var fileExtension = Path.GetExtension(image.FileName).ToLower();

            if (!IsAllowedExtension(fileExtension))
            {
                return BadRequest("Only JPG and PNG files are allowed.");
            }
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Ok(new { filepath = filePath });
      


        }

        [HttpPost("GenerateCaricature/{type}")]
        public async Task<IActionResult> GenerateCaricature(IFormFile image, string type)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image provided.");
            }
            var fileExtension = Path.GetExtension(image.FileName).ToLower();
            if (!IsAllowedExtension(fileExtension))
            {
                return BadRequest("Only JPG and PNG files are allowed.");
            }
            var fileName = Guid.NewGuid().ToString();
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            var apiUrl = "https://www.ailabapi.com/api/portrait/effects/portrait-animation";
            var request_caracrature = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request_caracrature.Headers.Add("ailabapi-api-key", apiKey);

            using (var content_caracrature = new MultipartFormDataContent())
            {
                content_caracrature.Add(new StreamContent(image.OpenReadStream()), "image", image.FileName);
                content_caracrature.Add(new StringContent(type), "type");

                request_caracrature.Content = content_caracrature;

                var response_caracrature = await _httpClient.SendAsync(request_caracrature);
                response_caracrature.EnsureSuccessStatusCode();

                string responseContent = await response_caracrature.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                if (responseObject != null && responseObject.Data != null && !string.IsNullOrEmpty(responseObject.Data.ImageUrl))
                {
                    string imageUrl = responseObject.Data.ImageUrl;
                    var caricatureImageFilePath = Path.Combine(uploadsFolderPath, $"{fileName}_{type}_caricature_image.jpg");

                    using (var imageResponse = await _httpClient.GetAsync(imageUrl))
                    using (var fileStream = new FileStream(caricatureImageFilePath, FileMode.Create))
                    {
                        await imageResponse.Content.CopyToAsync(fileStream);

                        await fileStream.FlushAsync();
                        fileStream.Close();
                    }

                    // Background removal
                    var apiUrl_bgremoval = "https://www.ailabapi.com/api/cutout/portrait/portrait-background-removal";
                    var request_bgremoval = new HttpRequestMessage(HttpMethod.Post, apiUrl_bgremoval);
                    request_bgremoval.Headers.Add("ailabapi-api-key", apiKey);

                    var content_bgremoval = new MultipartFormDataContent();

                    using (var fileStreamContent = new FileStream(caricatureImageFilePath, FileMode.Open))
                    {
                        content_bgremoval.Add(new StreamContent(fileStreamContent), "image", "response_image.jpg");
                        content_bgremoval.Add(new StringContent("whiteBK"), "return_form");

                        request_bgremoval.Content = content_bgremoval;

                        var response_bgremoval = await _httpClient.SendAsync(request_bgremoval);
                        response_bgremoval.EnsureSuccessStatusCode();

                        string responseContentRemoveBg = await response_bgremoval.Content.ReadAsStringAsync();
                        var responseObjectRemoveBg = JsonConvert.DeserializeObject<ApiResponse>(responseContentRemoveBg);

                        if (responseObjectRemoveBg != null && responseObjectRemoveBg.Data != null && !string.IsNullOrEmpty(responseObjectRemoveBg.Data.ImageUrl))
                        {
                            string finalImageUrl = responseObjectRemoveBg.Data.ImageUrl;
                            var finalResponseImageFilePath = Path.Combine(uploadsFolderPath, $"{fileName}_{type}_final_image.jpg");

                            using (var imageResponseRemoveBg = await _httpClient.GetAsync(finalImageUrl))
                            using (var fileStream2 = new FileStream(finalResponseImageFilePath, FileMode.Create))
                            {
                                await imageResponseRemoveBg.Content.CopyToAsync(fileStream2);
                            }
                            CaricatureResponse caricatureResponse = new CaricatureResponse
                            {
                                CaricatureImage = responseObject.Data.ImageUrl,
                                FinalImage = responseObjectRemoveBg.Data.ImageUrl,
                                FinalImagePath = finalResponseImageFilePath,
                                CaricatureImagePath = caricatureImageFilePath
                            };
                            return Ok(caricatureResponse);
                        }
                    }

                }

                return BadRequest("Failed to process the image.");
            }

        }
        private bool IsAllowedExtension(string fileExtension)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            return allowedExtensions.Contains(fileExtension);
        }


    }
}















