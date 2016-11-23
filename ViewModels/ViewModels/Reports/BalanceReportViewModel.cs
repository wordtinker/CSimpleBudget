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
        public DateTime Date { get; set; }
        public decimal Change { get; set; }
        public decimal Total { get; set; }
        public bool IsNegative { get { return Total < 0; } }
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
            // Add all transactions for a selected period
            List<Transaction> transactions = Core.Instance.GetTransactions(SelectedYear, SelectedMonth);
            transactions.Reverse();
            foreach (Transaction tr in transactions)
            {
                // filter out transaction before the last transaction date
                if (tr.Date > lastTransactionDate)
                {
                    startingBalance += tr.Amount;
                    BalanceRecords.Add(new BalanceItem
                    {
                        Date = tr.Date,
                        Change = tr.Amount,
                        Total = startingBalance,
                        Origin = "Transaction",
                        Category = (new CategoryNode(tr.Category)).FullName
                    });
                }
            }
            // Add all predictors for a selected period.
            DateTime actualDate = DateTime.Today;
            // Repeat for every month before selected
            while (actualDate.Month <= SelectedMonth && actualDate.Year <= SelectedYear)
            {
                foreach (Prediction pr in Predictor.Predict(actualDate))
                {
                    startingBalance += pr.Amount;
                    BalanceRecords.Add(new BalanceItem
                    {
                        Date = pr.Date,
                        Change = pr.Amount,
                        Total = startingBalance,
                        Origin = "Prediction",
                        Category = (new CategoryNode(pr.Category)).FullName
                    });
                }
                actualDate = actualDate.AddMonths(1);
            }
        }

        // ctor
        public BalanceReportViewModel()
        {
            UpdateBalance();
        }
    }
}
