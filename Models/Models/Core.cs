using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Models
{
    public class Core : BindableBase
    {
        private static readonly Core instance = new Core();
        private FileReader storage;
        private Account currentAccount;

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
                // TODO Fixme. core or DB?
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
    }
}
