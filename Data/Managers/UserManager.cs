using AutoMapper;
using Azure.Storage.Blobs;
using Data.Contract.Request;
using Data.Contract.Response;
using Data.Domain;
using Data.Enums;
using Data.Extensions;
using Data.Helpers;
using Data.Repository.EntityFilters;
using Data.Repository.EntityModels;
using Data.Repository.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Managers
{

    public interface IUserManager
    {
        Task<UserResponse> SaveAsync(UserRequest request);
        Task<UserResponse> UpdateAsync(UserRequest request);
        Task<UserResponse> GetAsync(UserFilter filter);
        Task<List<UserResponse>> SearchAsync(UserFilter filter);
        Task<bool> DeleteAsync(UserFilter filter);
        Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<ResultData> UserSearchAsync(UserFilter userFilter);
        Task<bool> SaveOrUpdateAsync(ApplicationUser model);
        Task<string> UploadImage([FromForm] IFormFile file, string id);
    }

    public class UserManager : IUserManager
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public UserManager(IUserService userService, IConfiguration configuration, IWebHostEnvironment environment, IMapper mapper, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
            _dbContext = dbContext;
            _environment = environment;
            var connectionString = configuration.GetConnectionString("StorageConnectionString");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = configuration["AppSettings:ContainerName"];
        }

        public async Task<UserResponse> GetAsync(UserFilter filter)
        {
            var entity = await _userService.GetAsync(filter);
            return _mapper.Map<UserResponse>(entity);
        }

        public async Task<UserResponse> SaveAsync(UserRequest request)
        {
            var entity = _mapper.Map<ApplicationUser>(request);
            var result = await _userService.SaveOrUpdateAsync(entity);
            return _mapper.Map<UserResponse>(result);
        }

        public async Task<bool> SaveOrUpdateAsync(ApplicationUser model)
        {
            var result = await _userService.SaveOrUpdateAsync(model);
            if (!string.IsNullOrEmpty(result.Id))
            {
                return true;
            }
            return false;
        }

        public async Task<List<UserResponse>> SearchAsync(UserFilter filter)
        {
            var result = await _userService.SearchAsync(filter);
            var users = result.Data as List<ApplicationUser>;
            return _mapper.Map<List<UserResponse>>(users);
        }

        public async Task<UserResponse> UpdateAsync(UserRequest request)
        {
            var entity = _mapper.Map<ApplicationUser>(request);
            var result = await _userService.SaveOrUpdateAsync(entity);
            return _mapper.Map<UserResponse>(result);
        }
        public async Task<bool> DeleteAsync(UserFilter filter)
        {
            bool retVal = false;
            var entity = await _userService.GetAsync(filter);
            if (entity != null)
            {
                retVal = await _userService.DeleteAsync(entity);
            }
            return retVal;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var changeResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!changeResult.Succeeded)
            {
                return false;
            }
            return changeResult.Succeeded;
        }

        public async Task<ResultData> UserSearchAsync(UserFilter filter)
        {
            var entity = await _userService.SearchAsync(filter);
            var resultData = new ResultData
            {
                Data = entity.Data as List<ApplicationUser>
            };
            return resultData;
        }

        public async Task<string> UploadImage([FromForm] IFormFile file, string id)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new Exception("Invalid file");
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                 var url =  await this.UploadFileAsync(file,fileName);
                return url;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();

                var blobClient = containerClient.GetBlobClient(fileName);
                await using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }
                return blobClient.Uri.ToString();
            }
            catch(Exception ex)
            {
                return null;
            }
           
        }
    }
}
