using Models;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ViewModels
{
    public class TransactionItem
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
        }
    }

    public class TransactionRollViewModel : BindableBase
    {
        private IUITransactionRollService service;
        public IEnumerable<TransactionItem> Transactions
        {
            get
            {
                return from tr in Core.Instance.Transactions
                       select new TransactionItem(tr);
            }
        }

        public bool DeleteTransaction(TransactionItem item)
        {
            return Core.Instance.DeleteTransaction(item.tr);
        }

        public void ShowTransactionEditor()
        {
            TransactionEditorViewModel vm = new TransactionEditorViewModel();
            service.ShowTransactionEditor(vm);
        }

        public void ShowTransactionEditor(TransactionItem item)
        {
            TransactionEditorViewModel vm = new TransactionEditorViewModel(item.tr);
            service.ShowTransactionEditor(vm);
        }

        public TransactionRollViewModel(IUITransactionRollService service)
        {
            this.service = service;

            Core.Instance.Transactions.ListChanged += (sender, e) =>
            {
                OnPropertyChanged(() => Transactions);
            };
        }

        public void Close()
        {
            // Cleanup selected account and transactions
            Core.Instance.CurrentAccount = null;
        }
    }
}
