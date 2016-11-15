using Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ViewModels
{
    public class BudgetRecordEditorViewModel : BindableBase
    {
        internal BudgetType BudgetType;

        private string monthName;
        private int month;
        private bool monthly;
        private bool point;
        private bool daily;
        private bool weekly;

        public IEnumerable<int> Years
        {
            get
            {
                int minYear, maxYear;
                Core.Instance.GetActiveBudgetYears(out minYear, out maxYear);
                return Enumerable.Range(minYear - 1, 5 + maxYear - minYear);
            }
        }
        public int Year { get; set; }
        public List<string> Months { get; } = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();
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
        public decimal Amount { get; set; }
        public IEnumerable<CategoryNode> Categories
        {
            get
            {
                return from c in Core.Instance.Categories
                       where c.Parent != null
                       select new CategoryNode(c);
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
                    BudgetType = BudgetType.Monthly;
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
                    BudgetType = BudgetType.Point;
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
                    BudgetType = BudgetType.Daily;
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
                    BudgetType = BudgetType.Weekly;
                    OnDay = 0;
                }
            }
        }
        public IEnumerable<int> Days { get; } = Enumerable.Range(1, 30);
        public IEnumerable<string> DaysOfWeek { get; } = Enum.GetNames(typeof(DayOfWeek));
        public int OnDay { get; set; }

        public BudgetRecordEditorViewModel()
        {
            Year = DateTime.Now.Year;
            Month = DateTime.Now.Month;
            Category = new CategoryNode((from c in Core.Instance.Categories where c.Parent != null select c).First());
            Monthly = true;
        }

        public BudgetRecordEditorViewModel(BudgetRecord record)
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
    }
}
