using Data.Domain;
using Data.Extensions;
using Data.Repository.EntityFilters;
using Data.Repository.EntityModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository.Services
{
    public interface ILogActivityService : IGenericRepository<Domain.LogActivity>
    {
        Task<EntityModels.ResultData> SearchAsync(LogActivitiesFilter filter = null);
        Task<Domain.LogActivity> GetAsync(LogActivitiesFilter filter = null);
    }
    public class LogActivityService : GenericRepository<Domain.LogActivity>, ILogActivityService
    {
        public readonly ApplicationDbContext _dbContext;
        public LogActivityService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Domain.LogActivity> GetAsync(LogActivitiesFilter filter = null)
        {
            ResultData result = await this.SearchAsync(filter);
            if (result.Total > 0)
                return (result.Data as List<Domain.LogActivity>).FirstOrDefault();

            return null;
        }
        public async Task<ResultData> SearchAsync(LogActivitiesFilter filter = null)
        {
            IQueryable<Domain.LogActivity> queryable = _dbContext.LogActivity.AsQueryable();

            if (filter != null)
            {
                if (filter.IsAsNoTracking)
                    queryable = _dbContext.LogActivity.AsNoTracking().AsQueryable();

                if (filter.EntityIdAsIntEqualTo.HasValue)
                {
                    queryable = queryable.Where(u => u.Id == filter.EntityIdAsIntEqualTo);
                }
            }

            List<Domain.LogActivity> records = await queryable.ToListAsync();

            return new ResultData
            {
                Data = records,
                Total = records.Count
            };
        }

        public override async Task<Domain.LogActivity> SaveAsync(LogActivity model, string processedBy)
        {
            LogActivity retVal;
            model.CreatedOn = DateTime.UtcNow.GetIndianCurrentDate();
            await base.CreateAsync(model);
            retVal = model;
            return retVal;
        }
    }
}
