using Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace ViewModels
{
    public class RecordItem : BindableBase
    {
        internal BudgetRecord record;
        internal BudgetType Type;

        private int month;
        private string monthName;
        private bool monthly;
        private bool point;
        private bool daily;
        private bool weekly;

        public string TypeName { get { return Type.ToString(); } }
        public int OnDay { get; set; }
        public decimal Amount { get; set; }
        public int Year { get; set; }
        public int Month
        {
            get { return month; }
            set
            {
                if (SetProperty(ref month, value))
                {
                    MonthName = DateTimeFormatInfo.CurrentInfo.MonthNames[value - 1];
                }
            }
        }
        public string MonthName
        {
            get { return monthName; }
            set
            {
                if (SetProperty(ref monthName, value))
                {
                    Month = DateTime.ParseExact(value, "MMMM", CultureInfo.CurrentCulture).Month;
                }
            }
        }
        public CategoryNode Category { get; set; }
        public bool Monthly
        {
            get { return monthly; }
            set
            {
                monthly = value;
                if (monthly)
                {
                    Type = BudgetType.Monthly;
                    OnDay = 0;
                }
            }
        }
        public bool Point
        {
            get { return point; }
            set
            {
                point = value;
                if (point)
                {
                    Type = BudgetType.Point;
                    OnDay = 1;
                }
            }
        }
        public bool Daily
        {
            get { return daily; }
            set
            {
                daily = value;
                if (daily)
                {
                    Type = BudgetType.Daily;
                    OnDay = 0;
                }
            }
        }
        public bool Weekly
        {
            get { return weekly; }
            set
            {
                weekly = value;
                if (weekly)
                {
                    Type = BudgetType.Weekly;
                    OnDay = 0;
                }
            }
        }

        /// <summary>
        /// Creates fake RecordItem with no real record behind
        /// </summary>
        public RecordItem()
        {
            this.record = null;
            Year = DateTime.Now.Year;
            Month = DateTime.Now.Month;
            Category = new CategoryNode((from c in Core.Instance.Categories where c.Parent != null select c).First());
            Monthly = true;
        }

        public RecordItem(BudgetRecord rec)
        {
            this.record = rec;
            Year = rec.Year;
            Month = rec.Month;
            Amount = rec.Amount;
            Category = new CategoryNode(rec.Category);

            switch (rec.Type)
            {
                case BudgetType.Monthly:
                    Monthly = true;
                    break;
                case BudgetType.Point:
                    Point = true;
                    OnDay = record.OnDay;
                    break;
                case BudgetType.Daily:
                    Daily = true;
                    break;
                case BudgetType.Weekly:
                    Weekly = true;
                    OnDay = record.OnDay;
                    break;
                default:
                    break;
            }
        }
    }

    public class BudgetManagerViewModel : BindableBase
    {
        private IUIBudgetWindowService windowService;
        private ICommand requestCopyFrom;
        private string selectedMonthName = DateTime.Now.ToString("MMMM");
        private int selectedYear = DateTime.Now.Year;

        public List<string> Months { get; } = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();
        public string SelectedMonthName
        {
            get
            {
                return selectedMonthName;
            }
            set
            {
                selectedMonthName = value;
                Core.Instance.SelectedMonth = DateTime.ParseExact(value, "MMMM", CultureInfo.CurrentCulture).Month;
            }
        }
        public IEnumerable<int> Years
        {
            get
            {
                int minYear, maxYear;
                Core.Instance.GetActiveBudgetYears(out minYear, out maxYear);
                return Enumerable.Range(minYear - 1, 5 + maxYear - minYear);
            }
        }
        public int SelectedYear {
            get
            {
                return selectedYear;
            }
            set
            {
                selectedYear = value;
                Core.Instance.SelectedYear = value;
            }
        }
        public IEnumerable<RecordItem> Records
        {
            get
            {
                return from rec in Core.Instance.Records
                       select new RecordItem(rec);
            }
        }

        public bool DeleteRecord(RecordItem item)
        {
            return Core.Instance.DeleteRecord(item.record);
        }

        public void ShowRecordEditor()
        {
            if (Core.Instance.Categories.Count == 0)
            {
                windowService.ShowMessage("Set categories first!");
            }
            else
            {
                BudgetRecordEditorViewModel vm = new BudgetRecordEditorViewModel();
                windowService.ShowBudgetRecordEditor(vm);
            }
        }

        public void ShowRecordEditor(RecordItem item)
        {
            BudgetRecordEditorViewModel vm = new BudgetRecordEditorViewModel(item.record);
            windowService.ShowBudgetRecordEditor(vm);
        }

        public ICommand RequestCopyFrom
        {
            get
            {
                return requestCopyFrom ??
                    (requestCopyFrom = new DelegateCommand(() =>
                    {
                        int monthToCopyFrom, yearToCopyFrom;
                        if (windowService.RequestMonthAndYear(out monthToCopyFrom, out yearToCopyFrom))
                        {
                            // monthToCopyFrom is zero based
                            Core.Instance.CopyRecords(monthToCopyFrom + 1, yearToCopyFrom);
                        }
                    }));
            }
        }

        //ctor
        public BudgetManagerViewModel(IUIBudgetWindowService windowService)
        {
            this.windowService = windowService;
            Core.Instance.Records.ListChanged += (sender, e) =>
            {
                OnPropertyChanged(() => Records);
            };

            Core.Instance.SelectedYear = SelectedYear;
            Core.Instance.SelectedMonth =
                DateTime.ParseExact(SelectedMonthName, "MMMM", CultureInfo.CurrentCulture).Month;
        }

        public void Close()
        {
            // Cleanup
            Core.Instance.SelectedYear = null;
            Core.Instance.SelectedMonth = null;
        }
    }
}
