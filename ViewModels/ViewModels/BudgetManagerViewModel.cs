using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace ViewModels
{
    public enum BudgetType
    {
        // TODO
        Monthly,
        Weekly
    }

    public class BudgetRecord
    {
        public Decimal Amount { get; set; }
        public string Category { get; set; }
        public BudgetType Type { get; set; }
        public int OnDay { get; set; }
        // TODO
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
                // TODO min, current-5 max,curr+5 stub from DB
                return new List<int> { 2014, 2015, 2016 };
            }
        }
        public int SelectedYear { get; set; } = DateTime.Now.Year;
        public ObservableCollection<BudgetRecord> Records { get; }



        //ctor
        public BudgetManagerViewModel(IUIBudgetWindowService windowService)
        {
            this.windowService = windowService;
            // TODO Stub
            Records = new ObservableCollection<BudgetRecord> {
                new BudgetRecord{ Amount=-500, Category = "Test", OnDay=1, Type=BudgetType.Monthly},
                new BudgetRecord{ Amount=-1000, Category = "Test2", OnDay=2, Type=BudgetType.Monthly}
            };
        }
    }
}
