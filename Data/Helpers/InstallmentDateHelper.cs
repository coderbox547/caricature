using Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Helpers
{
    public class InstallmentDateHelper
    {
        public List<DateTime> GetInstallmentDates(DateTime startDate, int totalDurationInMonths)
        {
            List<DateTime> _installmentDates = new List<DateTime>();

            for (int i = 0; i < totalDurationInMonths; i++)
            {
                _installmentDates.Add(startDate.AddMonths(i));
            }

            return _installmentDates;
        }

        public DateTime GetInstallmentDayOfCurrentMonth(DateTime startDate, int totalDurationInMonths)
        {

            List<DateTime> _installmentDates = this.GetInstallmentDates(startDate, totalDurationInMonths);

            DateTime currentDate = default(DateTime).GetIndianCurrentDate();

            DateTime targetDate = _installmentDates.FirstOrDefault(x => x.Month == currentDate.Month && x.Year == currentDate.Year);

            return targetDate;
        }

        public int FindInstallmentNumberFromInstallementDate(DateTime StartDate, int lDurationInMonths, DateTime installmentDate)
        {
            List<DateTime> _dates = this.GetInstallmentDates(StartDate, lDurationInMonths);
            return _dates.FindIndex(x => x == installmentDate) + 1;
        }
    }
}
