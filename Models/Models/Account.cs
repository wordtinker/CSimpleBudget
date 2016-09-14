
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
                Core.Instance.UpdateAccount(this);
            }
        }
        public string Type
        {   
            get { return accType; }
            set
            {
                SetProperty(ref accType, value);
                Core.Instance.UpdateAccount(this);
            }
        }
        public decimal Balance
        {
            get { return balance; }
            set
            {
                SetProperty(ref balance, value);
                Core.Instance.UpdateAccount(this);
            }
        }
        public bool Closed
        {
            get { return closed; }
            set
            {
                SetProperty(ref closed, value);
                Core.Instance.UpdateAccount(this);
            }
        }
        public bool Excluded
        {
            get { return excluded; }
            set
            {
                SetProperty(ref excluded, value);
                Core.Instance.UpdateAccount(this);
            }
        }
    }
}
