using Prism.Mvvm;
using System.Collections.Generic;
using Models;
using System.Linq;

namespace ViewModels
{
    public class Item
    {
        private Account account;

        public string Name { get { return account.Name; }}
        public int Type
        {
            get { return (int)account.Type; }
            set
            {
                account.Type = (AccType)value;
                Core.Instance.UpdateAccount(account);
            }
        }
        public bool Closed
        {
            get { return account.Closed; }
            set
            {
                account.Closed = value;
                Core.Instance.UpdateAccount(account);
            }
        }
        public bool Excluded
        {
            get { return account.Excluded; }
            set
            {
                account.Excluded = value;
                Core.Instance.UpdateAccount(account);
            }
        }

        public Item(Account acc)
        {
            this.account = acc;
        }
    }


    public class AccountsViewModel : BindableBase
    {
        public IEnumerable<string> AccTypes
        {
            get
            {
                // TODO MVVM violation redo with proper enum support
                return new List<string> { "Bank", "Cash", "Credit card" };
                //return Enum.GetValues(typeof(AccType)).Cast<AccType>().Select(x => x.ToString()).ToList();
            }
        }

        public Item SelectedAccount { get; set; }

        public IEnumerable<Item> Accounts
        {
            get
            {
                return from acc in Core.Instance.Accounts
                       select new Item(acc);
            }
        }

        public void UpdateSelection(Item selectedItem)
        {
            SelectedAccount = selectedItem;
            OnPropertyChanged(() => SelectedAccount);
        }

        // TODO
        //public ICommand AddAccount
        //{
        //    get
        //    {
        //        // TODO !!!
        //    }
        //}
    }
}
