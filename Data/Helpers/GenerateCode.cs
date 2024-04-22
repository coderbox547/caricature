using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Helpers
{
    public class GenerateCode
    {
        public static string GenerateGuidString()
        {
            return Guid.NewGuid().ToString().Substring(0, 16);
        }
    }
}
