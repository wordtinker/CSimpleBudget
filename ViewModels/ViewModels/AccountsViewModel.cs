using Prism.Mvvm;
using System.Collections.Generic;
using Models;
using System.Linq;

namespace ViewModels
{
    public class Item : BindableBase
    {
        public Account account;

        public string Name { get { return account.Name; }}
        public int Type
        {
            get { return (int)account.Type; }
            set
            {
                account.Type = (AccType)value;
            }
        }
        public string SType
        {
            get
            {
                return AccountsViewModel.AccTypes.ElementAt(Type);
            }
        }
        public bool Closed
        {
            get { return account.Closed; }
            set
            {
                account.Closed = value;
            }
        }
        public bool Excluded
        {
            get { return account.Excluded; }
            set
            {
                account.Excluded = value;
            }
        }

        public decimal Balance
        {
            get { return account.Balance; }
        }

        public Item(Account acc)
        {
            this.account = acc;
        }
    }


    public class AccountsViewModel : BindableBase
    {
        public static IEnumerable<string> AccTypes
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

        public bool AddAccount(string accName)
        {
            return Core.Instance.AddAccount(accName);
        }

        public bool DeleteAccount(Item item)
        {
            return Core.Instance.DeleteAccount(item.account);
        }

        public AccountsViewModel()
        {
            Core.Instance.Accounts.ListChanged += (sender, e) =>
            {
                OnPropertyChanged(() => Accounts);
            };
        }
    }
}
