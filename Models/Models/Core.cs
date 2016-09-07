using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;

namespace Models
{
    public class Core : BindableBase
    {
        private static readonly Core instance = new Core();
        private FileReader storage;
        private Account currentAccount;
        private int? currentYear;
        private int? currentMonth; // 1-based

        public static Core Instance
        {
            get
            {
                return instance;
            }
        }

        public FileReader Storage
        {
            get
            {
                return storage;
            }
            set
            {
                storage = value;

                Accounts.Clear();
                Categories.Clear();
                if (storage != null)
                {
                    storage.SelectAccounts().ForEach(Accounts.Add);
                    storage.SelectCategories().ForEach(Categories.Add);
                }
            }
        }

        // TODO Stub
        // ADD GUI for adding, deleting, and retrieving Acc Types
        public List<string> AccountTypes { get; } = new List<string> { "Bank", "Cash", "Credit Card" };

        public BindingList<Account> Accounts { get; } = new BindingList<Account>();

        public void UpdateAccount(Account acc)
        {
            Storage.UpdateAccount(acc);
        }

        public bool AddAccount(string accName, string accType)
        {
            Account newAcc;
            if (storage.AddAccount(accName, accType, out newAcc))
            {
                Accounts.Add(newAcc);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteAccount(Account account)
        {
            if (storage.DeleteAccount(account))
            {
                Accounts.Remove(account);
                return true;
            }
            else
            {
                return false;
            }
        }

        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        public bool AddCategory(string name, Category parent)
        {
            Category newCat;
            if (storage.AddCategory(name, parent, out newCat))
            {
                Categories.Add(newCat);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteCategory(Category cat)
        {
            if (storage.DeleteCategory(cat))
            {
                cat.Parent?.Children.Remove(cat);
                Categories.Remove(cat);
                return true;
            }
            else
            {
                return false;
            }
        }

        public BindingList<Transaction> Transactions { get; } = new BindingList<Transaction>();
        public Account CurrentAccount
        {
            get
            {
                return currentAccount;
            }
            set
            {
                currentAccount = value;
                Transactions.Clear();
                if (currentAccount != null)
                {
                    storage.SelectTransactions(currentAccount).ForEach(Transactions.Add);
                }
            }
        }

        public bool DeleteTransaction(Transaction transaction)
        {
            if (storage.DeleteTransaction(transaction))
            {
                Transactions.Remove(transaction);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddTransaction(DateTime date, decimal amount, string info, Category category)
        {
            Transaction newTr;
            if (storage.AddTransaction(CurrentAccount, date, amount, info, category, out newTr))
            {
                Transactions.Add(newTr);
            }
        }


        public void UpdateTransaction(Transaction tr, DateTime date, decimal amount, string info, Category category)
        {
            storage.UpdateTransaction(tr, date, amount, info, category);
        }

        public int? CurrentYear
        {
            get
            {
                return currentYear;
            }
            set
            {
                if (SetProperty(ref currentYear, value))
                {
                    Records.Clear();
                    if (CurrentYear != null && CurrentMonth != null)
                    {
                        storage.SelectRecords(CurrentYear.Value, CurrentMonth.Value).ForEach(Records.Add);
                    }
                }
            }
        }
        public int? CurrentMonth
        {
            get
            {
                return currentMonth;
            }
            set
            {
                if (SetProperty(ref currentMonth, value))
                {
                    Records.Clear();
                    if (CurrentYear != null && CurrentMonth != null)
                    {
                        storage.SelectRecords(CurrentYear.Value, CurrentMonth.Value).ForEach(Records.Add);
                    }
                }
            }
        }
        public BindingList<BudgetRecord> Records { get; } = new BindingList<BudgetRecord>();

        public bool DeleteRecord(BudgetRecord record)
        {
            if (storage.DeleteRecord(record))
            {
                Records.Remove(record);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
