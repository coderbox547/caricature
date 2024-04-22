using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository.EntityFilters
{
    public class UserFilter:BaseFetchFilter
    {
        public Enums.UserRole? Role { get; set; }
    }
}
