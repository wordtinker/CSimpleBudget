using Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ViewModels
{
    public class CategoriesReportViewModel : BindableBase
    {
        private int selectedYear = DateTime.Now.Year;
        private CategoryNode selectedCategory;

        public IEnumerable<int> Years
        {
            get
            {
                int minYear, maxYear;
                // TODO budget years are returned now, can lose some periods.
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
                    UpdateBars();
                }
            }
        }
        public List<CategoryNode> Categories { get; private set; }
        public CategoryNode SelectedCategory
        {
            get
            {
                return selectedCategory;
            }
            set
            {
                if (SetProperty(ref selectedCategory, value))
                {
                    UpdateBars();
                }
            }
        }
        public ObservableCollection<BudgetBar> Bars { get; } = new ObservableCollection<BudgetBar>();
        public ObservableCollection<TransactionItem> Transactions { get; } = new ObservableCollection<TransactionItem>();

        private void UpdateBars()
        {
            Bars.Clear();
            for (int month = 1; month < 13; month++)
            {
                Core.Instance.GetSpendings(SelectedYear, month).ForEach((s) =>
                {
                    if (s.Category == SelectedCategory.category)
                    {
                        Bars.Add(new BudgetBar(s));
                    }
                });
            }
        }

        public void UpdateTransactions(BudgetBar bar)
        {
            Transactions.Clear();
            Core.Instance.GetTransactions(SelectedYear, bar.Month, SelectedCategory.category).ForEach((tr) =>
            {
                Transactions.Add(new TransactionItem(tr));
            });
        }

        // ctor
        public CategoriesReportViewModel()
        {
            Categories = (from c in Core.Instance.Categories
                         where c.Parent != null
                         select new CategoryNode(c)).ToList();
            SelectedCategory = Categories[0];
            UpdateBars();
        }
    }
}
