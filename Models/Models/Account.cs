
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
            internal set
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
            internal set
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
