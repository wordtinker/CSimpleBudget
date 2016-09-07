using Prism.Mvvm;

namespace Models
{
    public enum BudgetType
    {
        Monthly,
        Point,
        Daily,
        Weekly
    }

    public class BudgetRecord : BindableBase
    {
        public decimal Amount { get; set; }
        public Category Category { get; set; }
        public BudgetType Type { get; set; }
        public int OnDay { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Id { get; internal set; }
    }
}
