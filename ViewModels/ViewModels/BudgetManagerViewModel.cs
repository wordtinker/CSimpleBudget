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
    public class RecordItem
    {
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
        private ICommand requestCopyFrom;
        private int selectedMonth = DateTime.Now.Month - 1;
        private int selectedYear = DateTime.Now.Year;

        public List<string> Months { get; } = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();
        public int SelectedMonth
        {
            get
            {
                return selectedMonth;
            }
            set
            {
                selectedMonth = value;
                Core.Instance.SelectedMonth = value + 1;
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
            Core.Instance.SelectedMonth = SelectedMonth + 1;
        }

        public void Close()
        {
            // Cleanup
            Core.Instance.SelectedYear = null;
            Core.Instance.SelectedMonth = null;
        }
    }
}
