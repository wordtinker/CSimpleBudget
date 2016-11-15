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

    /// <summary>
    /// Container type for budget record.
    /// </summary>
    public class RecordItem : BindableBase
    {
        internal BudgetRecord record;

        public decimal Amount { get { return record.Amount; } }
        public CategoryNode Category { get { return new CategoryNode(record.Category); } }
        public string TypeName { get { return record.Type.ToString(); } }
        public string OnDayText
        {
            get
            {
                if(record.Type == BudgetType.Monthly || record.Type == BudgetType.Daily)
                {
                    return string.Empty;
                }
                if (record.Type == BudgetType.Weekly)
                {
                    return ((DayOfWeek)record.OnDay).ToString();
                }
                return record.OnDay.ToString();
            }
        }

        public RecordItem(BudgetRecord rec)
        {
            this.record = rec;
            rec.PropertyChanged += (sender, e) =>
            {
                // Raise all properties changed.
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

        /// <summary>
        /// Clears list of records and loads new list for selected month and year.
        /// </summary>
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

        /// <summary>
        /// Raises request for editing control for new budget record.
        /// </summary>
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
                    BudgetRecord newRecord;
                    if (Core.Instance.AddRecord(
                        vm.Amount, vm.Category.category,
                        vm.BudgetType, vm.OnDay,
                        vm.Month, vm.Year, out newRecord))
                    {
                        if (newRecord.Month == SelectedMonth && newRecord.Year == SelectedYear)
                        {
                            Records.Add(new RecordItem(newRecord));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises request for editing control for existing budget record.
        /// </summary>
        /// <param name="item"></param>
        public void ShowRecordEditor(RecordItem item)
        {
            BudgetRecordEditorViewModel vm = new BudgetRecordEditorViewModel(item.record);
            if (windowService.ShowBudgetRecordEditor(vm) == true)
            {
                if (Core.Instance.UpdateRecord(
                    item.record, vm.Amount, vm.Category.category,
                    vm.BudgetType, vm.OnDay,
                    vm.Month, vm.Year))
                {
                    if (vm.Month != SelectedMonth || vm.Year != selectedYear)
                    {
                        Records.Remove(item);
                    }
                }
            }
        }

        /// <summary>
        /// Raises request for managing control to retrieve month and year 
        /// amd copy all budget records to selected month and year.
        /// </summary>
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
