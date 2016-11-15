using Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public string OnDayText { get
            {
                if (Monthly || Daily)
                {
                    return string.Empty;
                }
                if (Weekly)
                {
                    return ((DayOfWeek)OnDay).ToString();
                }
                return OnDay.ToString();
            }
        }
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

        private void Update()
        {
            Year = record.Year;
            Month = record.Month;
            Amount = record.Amount;
            Category = new CategoryNode(record.Category);

            switch (record.Type)
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
            Update();
            rec.PropertyChanged += (sender, e) =>
            {
                // Raise all properties changed.
                Update();
                OnPropertyChanged(string.Empty);
            };
        }
    }

    public class BudgetManagerViewModel : BindableBase
    {
        private IUIBudgetWindowService windowService;
        private ICommand requestCopyFrom;
        private string selectedMonthName = DateTime.Now.ToString("MMMM");
        private int selectedYear = DateTime.Now.Year;

        public ObservableCollection<RecordItem> Records { get; } = new ObservableCollection<RecordItem>();
        public List<string> Months { get; } = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();
        private int SelectedMonth {
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
                    UpdateRecords();
                }
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
                if (SetProperty(ref selectedYear, value))
                {
                    UpdateRecords();
                }
            }
        }

        private void UpdateRecords()
        {
            Records.Clear();
            Core.Instance.GetRecords(SelectedYear, SelectedMonth).ForEach((rec) =>
            {
                Records.Add(new RecordItem(rec));
            });
        }

        public bool DeleteRecord(RecordItem item)
        {
            if (Core.Instance.DeleteRecord(item.record))
            {
                Records.Remove(item);
                return true;
            }
            else
            {
                return false;
            }
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
                if (windowService.ShowBudgetRecordEditor(vm) == true)
                {
                    RecordItem newRecordItem = vm.BudgetRecord;

                    BudgetRecord newRecord;
                    if (Core.Instance.AddRecord(
                        newRecordItem.Amount, newRecordItem.Category.category,
                        newRecordItem.Type, newRecordItem.OnDay,
                        newRecordItem.Month, newRecordItem.Year, out newRecord))
                    {
                        if (newRecord.Month == SelectedMonth && newRecord.Year == SelectedYear)
                        {
                            Records.Add(new RecordItem(newRecord));
                        }
                    }
                }
            }
        }

        public void ShowRecordEditor(RecordItem item)
        {
            BudgetRecordEditorViewModel vm = new BudgetRecordEditorViewModel(item.record);
            if (windowService.ShowBudgetRecordEditor(vm) == true)
            {
                RecordItem editedRecordItem = vm.BudgetRecord;

                if (Core.Instance.UpdateRecord(
                    item.record, editedRecordItem.Amount, editedRecordItem.Category.category,
                    editedRecordItem.Type, editedRecordItem.OnDay,
                    editedRecordItem.Month, editedRecordItem.Year))
                {
                    if (editedRecordItem.Month != SelectedMonth || editedRecordItem.Year != selectedYear)
                    {
                        Records.Remove(item);
                    }
                }
            }
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
                            Core.Instance.CopyRecords(monthToCopyFrom, yearToCopyFrom, SelectedMonth, SelectedYear);
                            UpdateRecords();
                        }
                    }));
            }
        }

        //ctor
        public BudgetManagerViewModel(IUIBudgetWindowService windowService)
        {
            this.windowService = windowService;
            UpdateRecords();
        }
    }
}
