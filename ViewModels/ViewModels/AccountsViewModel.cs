using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows.Input;
using Models;
using System;
using System.Linq;

namespace ViewModels
{
    public class AccountsViewModel : BindableBase
    {
        public IEnumerable<string> AccTypes
        {
            get
            {
                // TODO !!!
                return Enum.GetValues(typeof(AccType)).Cast<AccType>().Select(x => x.ToString()).ToList();
            }
        }

        public IEnumerable<Account> Accounts
        {
            get
            {
                return Core.Instance.Accounts;
                // TODO stub
            }
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
