using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Managers
{
    using System.Data;
    using System.IdentityModel.Tokens.Jwt;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Data.Contract.Request;
    using Data.Contract.Response;
    using Data.Domain;
    using Data.Enums;
    using Data.Helpers;
    using Data.Repository.EntityModels;
    using Data.Repository.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

    public interface IAccountManager
    {
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task LogoutAsync();
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    }

    public class AccountManager : IAccountManager
    {
        private string secretKey;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IIdentityTokenService _identityTokenService;
        private readonly IUserService _userService;


        public AccountManager(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration,
            ApplicationDbContext dbContext, IIdentityTokenService identityTokenService, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _identityTokenService = identityTokenService;
            _userService = userService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            ValidationHelper.Validate(loginRequest, Validateuser);
            var user = await _userManager.FindByNameAsync(loginRequest.UserName);

            if (user != null)
            {
                IList<string> existingRoles = await _userManager.GetRolesAsync(user);
                //var role = existingRoles.FirstOrDefault();

                //if (role == UserRole.Admin.ToString())
                //{
                //    return new LoginResponse()
                //    {
                //        Message = "Invalid Login",
                //    };
                //}

                bool isValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

                if (isValid)
                {
                    var token = await GenerateJwtTokenAsync(user);
                    LoginResponse loginResponse = new LoginResponse()
                    {
                        Token = token.Token,
                        UserName = user.UserName,
                        UserLogo = user.UserLogo,
                        ID = user.Id,
                        Name = user.Name,
                        RefreshToken = token.RefreshToken
                    };

                    return loginResponse;
                }
            }

            return new LoginResponse()
            {
                Message = "Invalid Login",
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        private async Task<AuthenticationResult> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var result = await _identityTokenService.IssueTokenAsync(user);
            return result;
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var principal = _identityTokenService.GetPrincipalFromExpiredToken(token);
            if (principal == null || !principal.HasClaim(c => c.Type == "id"))
            {
                return new AuthenticationResult { Errors = new[] { "Invalid Token" } };
            }

            var userId = principal.FindFirstValue("id");

            var savedRefreshToken = _identityTokenService.GetSavedRefreshTokens(userId, refreshToken);

            if (savedRefreshToken.RefreshTok != refreshToken)
            {
                return new AuthenticationResult {Errors = new[] { ("Invalid attempt!") } };
            }

            var user = await _userService.GetAsync(new Repository.EntityFilters.UserFilter { EntityIdAsStringEqualTo = userId});

            var newAuthTokenResult = await _identityTokenService.IssueTokenAsync(user);

            if (!newAuthTokenResult.Success)
            {
                return new AuthenticationResult { Errors = new[] { ("Invalid attempt!") } };
            }

            RefreshToken obj = new RefreshToken
            {
                RefreshTok = newAuthTokenResult.RefreshToken,
                UserId = userId
            };

            _identityTokenService.DeleteUserRefreshTokens(userId, refreshToken);
            _identityTokenService.AddUserRefreshTokens(obj);

            return newAuthTokenResult; 
        }

        private List<string> Validateuser(LoginRequest model)
        {
            List<string> errors = new List<string>();
            if (String.IsNullOrEmpty(model.UserName))
            {
                errors.Add("UserName  is required");
            }
            if (String.IsNullOrEmpty(model.Password))
            {
                errors.Add("Password is required");
            }



            return errors;
        }


    }
}


