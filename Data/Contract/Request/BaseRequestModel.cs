using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contract.Request
{
    public class BaseRequestModel
    {
        public int? Id { get; set; }      
    
        public DateTime? ModifiedDateOnUTC { get; set; }
        public string ModifiedBy { get; set; }
    }
}
