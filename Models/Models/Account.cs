
namespace Models
{
    public enum AccType
    {
        Bank,
        Cash,
        CreditCard
    }

    public class Account
    {
        public string Name { get; set; }
        public AccType Type { get; set; }
        public bool Closed { get; set; }
        public bool Excluded { get; set; }
    }
}
