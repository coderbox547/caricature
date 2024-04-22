using AutoMapper;
using Data.Contract.Request;
using Data.Contract.Response;
using Data.Domain;
using Data.Enums;
using Data.Extensions;
using Data.Helpers;
using Data.Repository.EntityFilters;
using Data.Repository.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Data.Managers
{

    public interface IDashboardManager
    {
        Task<GetResponse<DashBoardStatResponse>> GetDashBoardStatAsync(string customerId);
    }

    public class DashboardManager : IDashboardManager
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public DashboardManager(ApplicationDbContext dbContext,  IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        public async Task<GetResponse<DashBoardStatResponse>> GetDashBoardStatAsync(string customerId)
        {
            throw new Exception();
        }
    }
}

