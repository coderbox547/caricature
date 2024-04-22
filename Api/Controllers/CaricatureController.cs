// Controllers/CaricatureController.cs

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CaricatureAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaricatureController : ControllerBase
    {

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
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

            return Ok(new { FolderName = folderName, FilePath = filePath });

        }

    }
}
