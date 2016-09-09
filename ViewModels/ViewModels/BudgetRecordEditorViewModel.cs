using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace ViewModels
{
    public class BudgetRecordEditorViewModel
    {
        private BudgetRecord record;

        public List<string> Months { get; } = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();
        public int SelectedMonth { get; set; } = DateTime.Now.Month - 1;
        public IEnumerable<int> Years { get; } = Core.Instance.GetActiveYears();
        public int SelectedYear { get; set; } = DateTime.Now.Year;
        public decimal Amount { get; set; }
        public IEnumerable<Node> Categories
        {
            get
            {
                return from c in Core.Instance.Categories
                       where c.Parent != null
                       select new Node(c);
            }
        }
        public Node Category { get; set; }
        public bool Monthly { get; set; }
        public bool Point { get; set; }
        public IEnumerable<int> Days { get; } = Enumerable.Range(1, 30);
        public int Day { get; set; } = 1;
        public bool Daily { get; set; }
        public bool Weekly { get; set; }
        public IEnumerable<string> DaysOfWeek { get; } = Enum.GetNames(typeof(DayOfWeek));
        public int DayOfWeek { get; set; } = 0;

        public ICommand UpdateRecord
        {
            get
            {
                throw new NotImplementedException();
                // TODO
            }
        }

        public BudgetRecordEditorViewModel()
        {
            Category = new Node((from c in Core.Instance.Categories where c.Parent != null select c).First());
            Monthly = true;
        }

        public BudgetRecordEditorViewModel(BudgetRecord record)
        {
            this.record = record;
            SelectedMonth = record.Month - 1;
            SelectedYear = record.Year;
            Amount = record.Amount;
            Category = new Node(record.Category);
            switch (record.Type)
            {
                case BudgetType.Monthly:
                    Monthly = true;
                    break;
                case BudgetType.Point:
                    Point = true;
                    Day = record.OnDay;
                    break;
                case BudgetType.Daily:
                    Daily = true;
                    break;
                case BudgetType.Weekly:
                    Weekly = true;
                    DayOfWeek = record.OnDay;
                    break;
                default:
                    break;
            }
        }
    }
}
