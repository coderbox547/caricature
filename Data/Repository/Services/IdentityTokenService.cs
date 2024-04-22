using Data.Domain;
using Data.Extensions;
using Data.Repository.EntityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository.Services
{
    public interface IIdentityTokenService
    {
        Task<AuthenticationResult> IssueTokenAsync(ApplicationUser user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        RefreshToken GetSavedRefreshTokens(string UserId, string refreshToken);
        void DeleteUserRefreshTokens(string UserId, string refreshToken);
        RefreshToken AddUserRefreshTokens(RefreshToken user);
    }
    public class IdentityTokenService : IIdentityTokenService
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;
       // private readonly JwtSetting _jwtSetting;
        private readonly ApplicationDbContext _dbContext;
        private string secretKey;

        public IdentityTokenService(UserManager<ApplicationUser> userManager,
          RoleManager<IdentityRole> roleMgr,
          ApplicationDbContext dbContext, IConfiguration configuration
          )
        {
            _userMgr = userManager;
            _roleMgr = roleMgr;
            // _jwtSetting = jwtSetting;
            _dbContext = dbContext;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<AuthenticationResult> IssueTokenAsync(ApplicationUser user)
        {
            var roles = await _userMgr.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
             new Claim(JwtRegisteredClaimNames.Sub, user.Email),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim(JwtRegisteredClaimNames.Email, user.Email),
              new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
             new Claim("id", user.Id),
         }),
                Expires = DateTime.UtcNow.GetIndianCurrentDate().AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = default(DateTime).GetIndianCurrentDate(),
                ExpiryDate = default(DateTime).GetIndianCurrentDate().AddMinutes(10),
                Token = tokenHandler.WriteToken(token)
            };

            // Generate a secure refresh token value
            refreshToken.RefreshTok = GenerateRefreshToken();

            await _dbContext.RefreshToken.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.RefreshTok,
            };
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }


            return principal;
        }



        public void DeleteUserRefreshTokens(string UserId, string refreshToken)
        {
            var item = _dbContext.RefreshToken.FirstOrDefault(x => x.UserId == UserId && x.RefreshTok == refreshToken);
            if (item != null)
            {
                _dbContext.RefreshToken.Remove(item);
                _dbContext.SaveChangesAsync();
            }
        }

        public RefreshToken GetSavedRefreshTokens(string UserId, string refreshToken)
        {
            return _dbContext.RefreshToken.FirstOrDefault(x => x.UserId == UserId && x.RefreshTok == refreshToken);
        }

        public RefreshToken AddUserRefreshTokens(RefreshToken user)
        {
            _dbContext.RefreshToken.Add(user);
            _dbContext.SaveChangesAsync();
            return user;
        }


    }
}
