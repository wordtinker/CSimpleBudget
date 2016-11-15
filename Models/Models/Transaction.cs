using Prism.Mvvm;
using System;

namespace Models
{
    public class Transaction : BindableBase
    {
        // Date of the transaction.
        private DateTime date;
        // Transaction value in decimal 0.00m format.
        private decimal amount;
        // Textual commentary for the transaction.
        private string info;
        // Transaction category. Every transaction has a category.
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

        // Unique transaction ID.
        internal int Id { get; }
        // Corresponding account reference.
        internal Account Account { get; }

        // ctor
        public Transaction(int id, Account acc)
        {
            this.Id = id;
            this.Account = acc;
        }
    }
}
