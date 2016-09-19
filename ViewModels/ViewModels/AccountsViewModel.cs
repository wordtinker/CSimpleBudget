using Prism.Mvvm;
using System.Collections.Generic;
using Models;
using System.Linq;

namespace ViewModels
{
    public class AccountItem : BindableBase
    {
        public Account account;

        public string Name { get { return account.Name; }}
        public string Type
        {
            get { return account.Type; }
            set
            {
                account.Type = value;
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

        public decimal Balance
        {
            get { return account.Balance; }
        }

        public bool Aggregated { get; internal set; }

        public AccountItem(Account acc)
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
                return Core.Instance.AccountTypes;
            }
        }

        public AccountItem SelectedAccount { get; set; }

        public IEnumerable<AccountItem> Accounts
        {
            get
            {
                return from acc in Core.Instance.Accounts
                       select new AccountItem(acc);
            }
        }

        public void UpdateSelection(AccountItem selectedItem)
        {
            SelectedAccount = selectedItem;
            OnPropertyChanged(() => SelectedAccount);
        }

        public bool AddAccount(string accName, string accType)
        {
            return Core.Instance.AddAccount(accName, accType);
        }

        public bool DeleteAccount(AccountItem item)
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
