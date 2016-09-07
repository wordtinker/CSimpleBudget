using Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ViewModels
{
    public class RecordItem
    {
        // TODO
        internal BudgetRecord record;

        public decimal Amount { get { return record.Amount; } }
        public string Category { get { return string.Format("{0}--{1}", record.Category.Parent?.Name, record.Category.Name); } }
        public string Type { get { return record.Type.ToString(); } }
        public int OnDay { get { return record.OnDay; } }

        public RecordItem(BudgetRecord rec)
        {
            this.record = rec;
        }
    }

    public class BudgetManagerViewModel : BindableBase
    {
        private IUIBudgetWindowService windowService;

        public List<string> Months { get; } = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();
        public int SelectedMonth { get; set; } = DateTime.Now.Month - 1;
        public List<int> Years
        {
            get
            {
                // TODO min-3 from DB
                int minYear = 2014;
                int maxYear = DateTime.Now.AddYears(3).Year;
                return Enumerable.Range(minYear, maxYear - minYear).ToList();
            }
        }
        public int SelectedYear { get; set; } = DateTime.Now.Year;
        public IEnumerable<RecordItem> Records
        {
            get
            {
                return from rec in Core.Instance.Records
                       select new RecordItem(rec);
            }
        }

        public void CurrenMonthChanged(int index)
        {
            Core.Instance.CurrentMonth = index + 1;
        }

        public void CurrentYearChanged(int year)
        {
            Core.Instance.CurrentYear = year;
        }

        public bool DeleteRecord(RecordItem item)
        {
            return Core.Instance.DeleteRecord(item.record);
        }

        //ctor
        public BudgetManagerViewModel(IUIBudgetWindowService windowService)
        {
            this.windowService = windowService;
            Core.Instance.Records.ListChanged += (sender, e) =>
            {
                OnPropertyChanged(() => Records);
            };

            Core.Instance.CurrentYear = SelectedYear;
            Core.Instance.CurrentMonth = SelectedMonth + 1;
        }

        public void Close()
        {
            // Cleanup
            Core.Instance.CurrentYear = null;
            Core.Instance.CurrentMonth = null;
        }
    }
}
