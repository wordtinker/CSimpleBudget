using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

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

        public ObservableCollection<Account> Accounts { get; } = new ObservableCollection<Account>();

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
            // TODO
        }

        public bool AddAccount(string accName)
        {
            // TODO stub
            Accounts.Add(new Account { Name = accName, Type = AccType.Bank });
            return true;
        }

        public bool DeleteAccount(Account account)
        {
            // TODO stub
            Accounts.RemoveAt(0);
            return true;
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
                    // TODO Stub
                    Accounts.Add(new Account { Name = "abc", Closed = true, Excluded = false, Type = AccType.Cash });
                    Accounts.Add(new Account { Name = "B account", Closed = false, Excluded = true, Type = AccType.Bank });
                }

                OnPropertyChanged(() => Categories);
            }
        }

        public bool AddCategory(string name, string parent)
        {
            if (storage.AddCategory(name, parent))
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
    }
}
