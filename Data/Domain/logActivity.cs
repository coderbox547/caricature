using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Extensions;

namespace Data.Domain
{
    public class LogActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Endpoint { get; set; }

        public string Method { get; set; }
        public string Headers { get; set; }

        public string Request { get; set; }
        public string Response { get; set; }

        public string RequestSentTime { get; set; }
        public string RequestCompleteTime { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow.GetIndianCurrentDate();

        public string Duration { get; set; }
        public long? LogExceptionId { get; set; }
    }
}
