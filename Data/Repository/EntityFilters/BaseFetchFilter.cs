
using System.Collections.Generic;

namespace Data.Repository.EntityFilters
{
    public abstract class BaseFetchFilter
    {
        public BaseFetchFilter()
        {
            EntityIdsAsIntEqualTo = new List<int>();
        }
        public PaginationInfo PaginationFilter { get; set; }
        public bool IsPaginationConfigure
        {
            get
            {
                return PaginationFilter != null;
            }
        }

       
      
        public bool IsCheckExistRecord { get; set; }

        public int? EntityIdAsIntEqualTo { get; set; }
        public IEnumerable<int> EntityIdsAsIntEqualTo { get; set; }
        public int? EntityIdAsIntNotEqualTo { get; set; }
        public string EntityIdAsStringEqualTo { get; set; }
        public string CreatedBy { get; set; }
        public string Keyword { get; set; }
        public bool DescendingOrderByCreatedOn { get; set; }
        public bool IsPublished { get; set; }

        public bool AscendingOrderByCreatedOn { get; set; }

        public bool IsAsNoTracking { get; set; }

    
    }

    public class PaginationInfo
    {
        public PaginationInfo()
        {
            //TODO: default values
            PageNumber = 1;
            PageSize = 100;
        }

        public PaginationInfo(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            //PageSize = pageSize > 100 ? 100 : pageSize;
        }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
