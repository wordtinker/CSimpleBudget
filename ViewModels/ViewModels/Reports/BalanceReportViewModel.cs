using Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace ViewModels
{
    public class BalanceItem
    {
        // TODO dummy
        public DateTime Date { get; set; }
        public decimal Change { get; set; }
        public decimal Total { get; set; }
        public string Origin { get; set; }
        public string Category { get; set; }
    }

    public class BalanceReportViewModel : BindableBase
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
                    UpdateBalance();
                }
            }
        }
        public IEnumerable<int> Years
        {
            get
            {
                // TODO check which years 
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
                    UpdateBalance();
                }
            }
        }
        public ObservableCollection<BalanceItem> BalanceRecords { get; } = new ObservableCollection<BalanceItem>();

        private void UpdateBalance()
        {
            BalanceRecords.Clear();

            DateTime lastTransactionDate;
            decimal startingBalance = Core.Instance.GetBalanceToDate(SelectedMonth, SelectedYear, out lastTransactionDate);
            // Add starting balance row
            BalanceRecords.Add(new BalanceItem
            {
                Date = lastTransactionDate,
                Change = 0m,
                Total = startingBalance,
                Origin = "Balance",
                Category = string.Empty
            });
            // TODO
        }

        // ctor
        public BalanceReportViewModel()
        {
            UpdateBalance();
        }
    }
}
