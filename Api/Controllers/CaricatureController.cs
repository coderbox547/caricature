﻿using Api.Models;
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

            var folderName = Guid.NewGuid().ToString();
            var folderPath = Path.Combine(uploadsFolderPath, folderName);
            Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, "originalImage.jpg");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return Ok(new { folderName = folderName });



        }

        [HttpPost("GenerateCaricature/{folderName}/{type}")]
        public async Task<IActionResult> GenerateCaricature(string folderName, string type)
        {
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderName);
            var existingfileType = Path.Combine(uploadsFolderPath, $"{type}.jpg");

            if (!Directory.Exists(uploadsFolderPath))
            {
                return NotFound("Folder not found");
            }
            if (System.IO.File.Exists(existingfileType))
            {
                return Ok(existingfileType);
            }
            var orginalFilePath = Path.Combine(uploadsFolderPath, "originalImage.jpg");

            if (!System.IO.File.Exists(orginalFilePath))
            {
                return NotFound("File not found.");
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
                    return Ok(imageUrl);
                }
                else
                {
                    return BadRequest("Error in Image processing.");
                }
            }
        }


        private bool IsAllowedExtension(string fileExtension)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            return allowedExtensions.Contains(fileExtension);
        }


    }
}















