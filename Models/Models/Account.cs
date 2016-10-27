
using Prism.Mvvm;

namespace Models
{
    public class Account : BindableBase
    {
        private string name;
        private string accType;
        private decimal balance;
        private bool closed;
        private bool excluded;

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
        /// <summary>
        /// Defines if the account is closed. Closed accounts do
        /// not contibute to totals. Closed accounts are considered
        /// in budgeting.
        /// </summary>
        public bool Closed
        {
            get { return closed; }
            set
            {
                SetProperty(ref closed, value);
            }
        }
        /// <summary>
        /// Defines if the account is considered in budgeting.
        /// Excluded account transactions will not be included in
        /// any budget report or forecast.
        /// </summary>
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
