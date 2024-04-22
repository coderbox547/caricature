using Data.Contract.Request;
using Data.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserManager _userManager;

        public UserController( IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("GetUserInformation")]
        public async Task<IActionResult> GetUserInformation()
        {
            var user = await _userManager.GetAsync(new Data.Repository.EntityFilters.UserFilter { EntityIdAsStringEqualTo = LogedInUserId });
            if (user == null)
            {
                return StatusCode(400, "user not found");
            }
            return Success(user);
        }
        [HttpPost("Upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var result = await _userManager.UploadImage(file,LogedInUserId);
            if(result!= null)
            {
                return Success(result);
            }
            else
            {
                return BadRequest("");
            }
        }

        [HttpPost("UpdateUserInformation")]
        public async Task<IActionResult> UpdateUserInformation([FromBody] UserRequest userRequest)
        {
            userRequest.Id = LogedInUserId;
            userRequest.Name = userRequest.FirstName;
            var user = await _userManager.UpdateAsync(userRequest);
            if (user == null)
            {
                return StatusCode(400, "user not found");
            }
            return Success(user);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = LogedInUserId;
            var changeResult = await _userManager.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (changeResult)
            {
                return Success(new { Message = "Password changed successfully." });
            }
            else
            {
                return StatusCode(400, "failed to change PassWord");
            }
        }
    }
}
