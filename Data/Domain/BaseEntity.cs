using Data.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Domain
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedDateOnIndianTime { get; set; } = default(DateTime).GetIndianCurrentDate();

        public string CreatedBy { get; set; }
        public DateTime? ModifiedDateOnUTC { get; set; }
        public string ModifiedBy { get; set; }

    }
}
