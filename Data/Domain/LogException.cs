using Data.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Domain
{
    public class LogException
    {
        [Key]
        public long Id { get; set; }

        public string Message { get; set; }

        [StringLength(5000)]
        public string StackTrace { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow.GetIndianCurrentDate();

        public ExceptionType ExceptionType { get; set; }
    }

    public enum ExceptionType
    {
        Identity = 1,
        Unauthorized,
        General
    }
}
