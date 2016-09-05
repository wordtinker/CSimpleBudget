using Prism.Mvvm;
using System;

namespace Models
{
    public class Transaction : BindableBase
    {
        private DateTime date;
        private decimal amount;
        private string info;
        private Category cat;

        public DateTime Date
        {
            get { return date; }
            set { SetProperty(ref date, value); }
        }
        public decimal Amount
        {
            get { return amount; }
            set { SetProperty(ref amount, value); }
        }
        public string Info
        {
            get { return info; }
            set { SetProperty(ref info, value); }
        }
        public Category Category
        {
            get { return cat; }
            set { SetProperty(ref cat, value); }
        }

        internal int Id { get; }
        internal Account Account { get; }

        public Transaction(int id, Account acc)
        {
            this.Id = id;
            this.Account = acc;

        }
    }
}
