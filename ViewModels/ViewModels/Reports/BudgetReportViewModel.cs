using Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace ViewModels
{
    public class BudgetReportViewModel : BindableBase
    {
        private string selectedMonthName = DateTime.Now.ToString("MMMM");
        private int selectedYear = DateTime.Now.Year;

        public List<string> Months { get; } = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();
        private int SelectedMonth
        {
            get
            {
                return DateTime.ParseExact(SelectedMonthName, "MMMM", CultureInfo.CurrentCulture).Month;
            }
        }
        public string SelectedMonthName
        {
            get
            {
                return selectedMonthName;
            }
            set
            {
                if (SetProperty(ref selectedMonthName, value))
                {
                    UpdateBars();
                }
            }
        }
        public IEnumerable<int> Years
        {
            get
            {
                int minYear, maxYear;
                Core.Instance.GetActiveBudgetYears(out minYear, out maxYear);
                return Enumerable.Range(minYear, 1 + maxYear - minYear);
            }
        }
        public int SelectedYear
        {
            get
            {
                return selectedYear;
            }
            set
            {
                if (SetProperty(ref selectedYear, value))
                {
                    UpdateBars();
                }
            }
        }
        public ObservableCollection<BudgetBar> Bars { get; } = new ObservableCollection<BudgetBar>();

        private void UpdateBars()
        {
            Bars.Clear();
            Core.Instance.GetSpendings(SelectedYear, SelectedMonth).ForEach((s) =>
            {
                Bars.Add(new BudgetBar(s));
            });
        }

        // ctor
        public BudgetReportViewModel()
        {
            UpdateBars();
        }
    }
}
