
using Prism.Mvvm;

namespace Models
{
    public class Account : BindableBase
    {
        // Account name. Name in NOT unique.
        private string name;
        // Corresponding account type reference. Acc type is of
        // object type shortened to string type.
        private string accType;
        // Total sum of all transactions.
        private decimal balance;
        // Flag "Closed"
        // Defines if the account is closed. Closed accounts do
        // not contibute to totals. Closed accounts are considered
        // in budgeting.
        // T - Account is closed.
        // F - Account is opened.
        private bool closed;
        // Flag "Out of Budget"
        // Defines if the account is considered in budgeting.
        // F - Transactions from that account are included in all budget
        // reports and forecasts.
        // T - Transactions from that account are excluded from all budget
        // reports and forecasts.
        private bool excluded;

        // Unique account ID.
        internal int Id { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                SetProperty(ref name, value);
            }
        }
        public string Type
        {   
            get { return accType; }
            set
            {
                SetProperty(ref accType, value);
            }
        }
        public decimal Balance
        {
            get { return balance; }
            set
            {
                SetProperty(ref balance, value);
            }
        }

        public bool Closed
        {
            get { return closed; }
            set
            {
                SetProperty(ref closed, value);
            }
        }

        public bool Excluded
        {
            get { return excluded; }
            set
            {
                SetProperty(ref excluded, value);
            }
        }
    }
}
