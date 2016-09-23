using Models;
using Prism.Mvvm;
using System;
using System.ComponentModel;

namespace ViewModels
{
    public class TransactionItem : BindableBase
    {
        internal Transaction tr;

        public string Date
        {
            get { return tr.Date.ToString("dd/MM/yyyy"); }
        }

        public string Value
        {
            get { return string.Format("{0:0.00}", tr.Amount); }
        }

        public string Info
        {
            get { return tr.Info; }
        }

        public string Category
        {
            get
            {
                Category category = tr.Category;
                return string.Format("{0}--{1}", category.Parent.Name, category.Name);
            }
        }

        public TransactionItem(Transaction tr)
        {
            this.tr = tr;
            tr.PropertyChanged += (sender, e) => {
                // Raise all properties changed.
                OnPropertyChanged(string.Empty);
            };
        }
    }

    public class TransactionRollViewModel : BindableBase
    {
        private IUITransactionRollService service;
        private Account account;

        public BindingList<TransactionItem> Transactions { get; } = new BindingList<TransactionItem>();

        public bool DeleteTransaction(TransactionItem item)
        {
            if (Core.Instance.DeleteTransaction(item.tr))
            {
                Transactions.Remove(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ShowTransactionEditor()
        {
            DateTime date;
            decimal amount;
            string info;
            CategoryNode catNode;

            TransactionEditorViewModel vm = new TransactionEditorViewModel();
            if (service.ShowTransactionEditor(vm, out date, out amount, out info, out catNode))
            {
                Transaction newTr;
                if (Core.Instance.AddTransaction(account, date, amount, info, catNode.category, out newTr))
                {
                    Transactions.Add(new TransactionItem(newTr));
                }
            }
        }

        public void ShowTransactionEditor(TransactionItem item)
        {
            DateTime date;
            decimal amount;
            string info;
            CategoryNode catNode;

            TransactionEditorViewModel vm = new TransactionEditorViewModel(item.tr);
            if (service.ShowTransactionEditor(vm, out date, out amount, out info, out catNode))
            {
                Core.Instance.UpdateTransaction(item.tr, date, amount, info, catNode.category);
            }
        }

        public TransactionRollViewModel(AccountItem accItem, IUITransactionRollService service)
        {
            this.service = service;
            this.account = accItem.account;
            Core.Instance.GetTransactions(account).ForEach((tr) =>
            {
                Transactions.Add(new TransactionItem(tr));
            });
        }
    }
}
