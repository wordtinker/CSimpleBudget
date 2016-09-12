using Prism.Mvvm;

namespace Models
{
    // TODO description
    public enum BudgetType
    {
        Monthly,
        Point,
        Daily,
        Weekly
    }

    public class BudgetRecord : BindableBase
    {
        private decimal amount;
        private Category category;
        private BudgetType budgetType;
        private int onDay;
        private int month;
        private int year;

        public decimal Amount
        {
            get { return amount; }
            set { SetProperty(ref amount, value); }
        }
        public Category Category
        {
            get { return category; }
            set { SetProperty(ref category, value); }
        }
        public BudgetType Type
        {
            get { return budgetType; }
            set { SetProperty(ref budgetType, value); }
        }
        public int OnDay
        {
            get { return onDay; }
            set { SetProperty(ref onDay, value); }
        }
        public int Month
        {
            get { return month; }
            set { SetProperty(ref month, value); }
        }
        public int Year
        {
            get { return year; }
            set { SetProperty(ref year, value); }
        }

        internal int Id { get; }

        public BudgetRecord(int id)
        {
            this.Id = id;
        }
    }
}
