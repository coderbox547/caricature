using Data.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public DateTime CreatedDateOnUTC { get; set; } = DateTime.UtcNow.GetIndianCurrentDate();
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDateOnUTC { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ValidUpto { get; set; }

        [NotMapped]
        public Enums.UserRole Role { get; set; }
        [NotMapped]
        public IList<string> Roles { get; set; }
        public string UserLogo { get; set; }
    }
}
