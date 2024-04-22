using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Extensions
{
    public static class DoubleExtensionMethod
    {
        public static string ToFigure(this double amount)
        {
            return String.Concat("₹ ", string.Format("{0:#,##0.00}", amount));
        }
    }
}
