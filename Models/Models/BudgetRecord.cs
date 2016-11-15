using Prism.Mvvm;

namespace Models
{
    /// <summary>
    /// Every budget record has a type that defines
    /// its behaviour in the budget forecasts.
    /// </summary>
    public enum BudgetType
    {
        // TODO check
        // TODO description of the timeframe. Budget is expired before date etc.
        Monthly, // One time spending, forecast spending is on the last day of the month
        Point, // One time spending on the specified day of the month
        Daily, // Spending is evenly divided among days of the month
        Weekly // Wekly spending on the specified day of the week
    }

    /// <summary>
    /// Single record for a planned loss or income of money.
    /// </summary>
    public class BudgetRecord : BindableBase
    {
        // Budget record value in decimal 0.00m format.
        // Value is set for whole month regardless of budget type.
        private decimal amount;
        // Budget category. Every budget has a category.
        private Category category;
        // Type of budget behaviour.
        private BudgetType budgetType;
        // Planned day of the transaction.
        // 0 is default value for Daily, Monthly
        // 1-30 range is available for Point
        // 0-6 range is available for Weekly. See DayOfWeek enum.
        private int onDay;
        // Planned month on which transaction will occur.
        private int month;
        // Planned year on which transaction will occur.
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
