
using Data.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repository.Services
{
    public interface ILogExceptionService : IGenericRepository<LogException>
    {
    }
    public class LogExceptionService : GenericRepository<LogException>, ILogExceptionService
    {
        private readonly ApplicationDbContext _dbContext;
        //private readonly IEventPublisher _eventPublisher;
        //, IEventPublisher eventPublisher
        public LogExceptionService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            //_eventPublisher = eventPublisher;
        }
    }
}
