using Data.Contract.Request;
using Data.Contract.Response;
using Data.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAccountManager _accountManager;

        public AuthController(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var loginResponse = await _accountManager.LoginAsync(loginRequest);
            if (loginResponse.UserName == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                return BadRequest("Invalid login");
            }
            return Success(loginResponse);
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _accountManager.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authResponse.Success)
            {
                throw  new Exception("Invalid attempt");
            }

            return Success(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
    }
}
