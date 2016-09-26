using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ViewModels
{
    public class BudgetRecordEditorViewModel
    {
        public RecordItem BudgetRecord { get; }

        public List<string> Months { get; } = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();
        public IEnumerable<int> Years
        {
            get
            {
                int minYear, maxYear;
                Core.Instance.GetActiveBudgetYears(out minYear, out maxYear);
                return Enumerable.Range(minYear - 1, 5 + maxYear - minYear);
            }
        }
        public IEnumerable<CategoryNode> Categories
        {
            get
            {
                return from c in Core.Instance.Categories
                       where c.Parent != null
                       select new CategoryNode(c);
            }
        }
        public IEnumerable<int> Days { get; } = Enumerable.Range(1, 30);
        public IEnumerable<string> DaysOfWeek { get; } = Enum.GetNames(typeof(DayOfWeek));

        public BudgetRecordEditorViewModel()
        {
            BudgetRecord = new RecordItem();
        }

        public BudgetRecordEditorViewModel(BudgetRecord record)
        {
            BudgetRecord = new RecordItem(record);
        }
    }
}
