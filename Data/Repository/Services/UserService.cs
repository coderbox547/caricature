using Data.Domain;
using Data.Enums;
using Data.Extensions;
using Data.Repository.EntityFilters;
using Data.Repository.EntityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository.Services
{

    public interface IUserService : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetAsync(UserFilter filter);
        Task<ResultData> SearchAsync(UserFilter filter);
        Task<bool> IsLicenseExpired(UserFilter filter);
    }
    public class UserService : GenericRepository<ApplicationUser>, IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : base(dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetAsync(UserFilter filter)
        {
            ResultData resultData = await this.SearchAsync(filter);
            if (resultData.Total > 0)
                return (resultData.Data as List<ApplicationUser>).FirstOrDefault();
            return null;
        }

        public async Task<ResultData> SearchAsync(UserFilter filter)
        {
            IQueryable<ApplicationUser> queryable = _dbContext.ApplicationUsers.AsQueryable();

            if (filter != null)
            {
                if (!String.IsNullOrEmpty(filter.EntityIdAsStringEqualTo))
                {
                    queryable = queryable.Where(x => x.Id == filter.EntityIdAsStringEqualTo);

                }
                if (filter.Role.HasValue)
                {
                    var roles = await _userManager.GetUsersInRoleAsync(filter.Role.ToString());
                    queryable = roles.AsQueryable();

                }

            }
            List<ApplicationUser> records = queryable.ToList();
            return new ResultData
            {
                Data = records,
                Total = records.Count,
            };
        }
        public async Task<bool> IsLicenseExpired(UserFilter filter)
        {
            bool retval = false;
            DateTime today = default(DateTime).GetIndianCurrentDate();
            var result = await this.GetAsync(filter);
            if (!string.IsNullOrEmpty(result.Id))
            {
                if (today > result.ValidUpto)
                {
                    retval = true;
                }
            }
            return retval;
        }

        public async override Task<ApplicationUser> SaveOrUpdateAsync(ApplicationUser model)
        {
            ApplicationUser retVal;
            if (String.IsNullOrEmpty(model.Id))
            {
                var user = await CreateUserAsync(model);
                retVal = user;
            }
            else
            {

                var user = await UpdateUserAsync(model);
                retVal = user;
            }
            return retVal;
        }

        private async Task<ApplicationUser> CreateUserAsync(ApplicationUser user)
        {
            try
            {
                var newUser = new ApplicationUser
                {
                    Email = user.Email,
                    UserName = user.Email,
                    Name = user.Name,
                    AccessFailedCount = user.AccessFailedCount,

                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    PasswordHash = user.Name + "321A"

                };

                var result = await _userManager.CreateAsync(newUser, newUser.PasswordHash);
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(newUser, user.Role.ToString()).GetAwaiter().GetResult();
                    return newUser;
                }
                else
                {
                    return new ApplicationUser();
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }
        private async Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
        {

            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser != null)
            {
                existingUser.Name = user.Name;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.UserLogo = user.UserLogo;

                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return existingUser;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("User Does not found");
            }


        }
    }
}
