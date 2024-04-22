
using System.Reflection;


#nullable enable
namespace Data.Repository.Services
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {

        Task<bool> CreateAsync(TEntity entity);

        Task<bool> BulkInsertAsync(List<TEntity> entities);

        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> BulkUpdateAsync(List<TEntity> entity);        

        Task<bool> BulkDeleteAsync(params object[] id);

        Task<TEntity> SaveOrUpdateAsync(TEntity model);
        Task<bool> DeleteAsync(TEntity model);
        Task<TEntity> GetByIdAsync(params object[] id);

        Task<TEntity> SaveAsync(TEntity model, string? processedBy = null);
    }

    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
     where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;


        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

        }





        public virtual async Task<bool> CreateAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            var recordsAffected = _dbContext.SaveChanges();

            if (recordsAffected <= 0)
            {
                var exception = new Exception("Insert operation failed");
                throw exception;
            }

            return true;
        }
        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            bool retVal = false;
            _dbContext.Set<TEntity>().Update(entity);
            var recordsAffected = _dbContext.SaveChanges();
            retVal = (recordsAffected > 0);

            if (!retVal)
            {
                throw new Exception("Update operation failed");
            }

            return retVal;
        }

        public virtual async Task<bool> BulkInsertAsync(List<TEntity> entities)
        {
            //await _dbContext.BulkInsertAsync<TEntity>(entities);
            //return true;

            await _dbContext.Set<TEntity>().AddRangeAsync(entities);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> BulkUpdateAsync(List<TEntity> entities)
        {
            List<bool> processingRecords = new List<bool>();

            _dbContext.Set<TEntity>().UpdateRange(entities);

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> BulkDeleteAsync(params object[] id)
        {
            List<TEntity> entityListToDelete = new List<TEntity>();
            foreach (var entityId in (List<int>)id[0])
            {
                var entity = await GetByIdAsync(entityId);
                if (entity != null)
                    entityListToDelete.Append(entity);
            }

            _dbContext.Set<TEntity>().RemoveRange(entityListToDelete);
            var deletedCount = await _dbContext.SaveChangesAsync();

            return deletedCount > 0;
        }

        public async Task<TEntity> GetByIdAsync(params object[] id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity> GetByCustomerIdAsync(string id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        private bool IsColumnExist(PropertyInfo[] collections, string propertyName)
        {
            return collections.Any(x => x.Name == propertyName);
        }

        public virtual Task<TEntity> SaveOrUpdateAsync(TEntity model)
        {
            // No need to write the code
            throw new Exception();
        }

        public async Task<bool> DeleteAsync(TEntity model)
        {
            _dbContext.Set<TEntity>().Remove(model);
            var deleteCount = await _dbContext.SaveChangesAsync();
            return deleteCount >0;
        }

        public virtual Task<TEntity> SaveAsync(TEntity model, string? processedBy = null)
        {
            throw new NotImplementedException();
        }
    }

}
