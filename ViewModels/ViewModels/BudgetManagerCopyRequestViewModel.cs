using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ViewModels
{
    /// <summary>
    /// VM for copy request window.
    /// </summary>
    public class BudgetManagerCopyRequestViewModel
    {
        public List<string> Months { get; } = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();
        /// <summary>
        /// This value is uses as "return" value of copy request.
        /// </summary>
        public int SelectedMonth
        {
            get
            {
                return DateTime.ParseExact(SelectedMonthName, "MMMM", CultureInfo.CurrentCulture).Month;
            }
        }
        public string SelectedMonthName{ get; set; } = DateTime.Now.ToString("MMMM");
        public IEnumerable<int> Years
        {
            get
            {
                int minYear, maxYear;
                Core.Instance.GetActiveBudgetYears(out minYear, out maxYear);
                return Enumerable.Range(minYear, 1 + maxYear - minYear);
            }
        }
        /// <summary>
        /// This value is used as "return" value of copy request.
        /// </summary>
        public int SelectedYear { get; set; } = DateTime.Now.Year;
    }
}
