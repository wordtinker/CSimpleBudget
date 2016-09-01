using Prism.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;

namespace Models
{
    public class Core : BindableBase
    {
        private static readonly Core instance = new Core();
        private FileReader storage;

        public static Core Instance
        {
            get
            {
                return instance;
            }
        }

        public BindingList<Account> Accounts { get; } = new BindingList<Account>();

        private List<Category> categories = new List<Category>();
        public List<Category> Categories {
            get
            {
                categories.Clear();
                if (storage != null)
                {
                    foreach (Category cat in storage.SelectCategories())
                    {
                        categories.Add(cat);
                    }
                }
                return categories;
            }
        }

        public void UpdateAccount(Account acc)
        {
            Storage.UpdateAccount(acc);
        }

        public bool AddAccount(string accName)
        {
            Account newAcc;
            if (storage.AddAccount(accName, out newAcc))
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
                if (storage != null)
                {
                    storage.SelectAccounts().ForEach(Accounts.Add);
                }

                OnPropertyChanged(() => Categories);
            }
        }

        public bool AddCategory(string name, string parent)
        {
            Category newCat;
            if (storage.AddCategory(name, parent, out newCat))
            {
                OnPropertyChanged(() => Categories);
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
                OnPropertyChanged(() => Categories);
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<Transaction> GetTransactions(Account acc)
        {
            return storage.SelectTransactions(acc);
        }
    }
}
