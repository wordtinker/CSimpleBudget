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
        private Dictionary<int, Category> categories;

        public string Date
        {
            get { return tr.Date.ToString("dd/MM/yyyy"); }
        }

        public string Value
        {
            get { return string.Format("{0:0.00}", tr.Value); }
        }

        public string Info
        {
            get { return tr.Info; }
        }

        public string Category
        {
            get
            {
                Category category = categories[tr.CategoryId];
                return string.Format("{0}--{1}", category.Parent.Name, category.Name);
            }
        }

        public TransactionItem(Transaction tr, Dictionary<int, Category> categories)
        {
            this.tr = tr;
            this.categories = categories;
        }
    }

    public class TransactionRollViewModel : BindableBase
    {
        private IUITransactionRollService service;
        private Dictionary<int, Category> catDictionary;
        
        public IEnumerable<TransactionItem> Transactions
        {
            get
            {
                return from tr in Core.Instance.Transactions
                       select new TransactionItem(tr, catDictionary);
            }
        }

        public bool DeleteTransaction(TransactionItem item)
        {
            return Core.Instance.DeleteTransaction(item.tr);
        }

        public TransactionRollViewModel(IUITransactionRollService service)
        {
            this.service = service;

            catDictionary = new Dictionary<int, Category>();
            (from cat in Core.Instance.Categories
             where cat.Parent != null
             select cat).ToList().ForEach(x => catDictionary.Add(x.Id, x));

            Core.Instance.Transactions.ListChanged += (sender, e) =>
            {
                OnPropertyChanged(() => Transactions);
            };

            // TODO empty cat for non budget accs?
        }

        public void Close()
        {
            // Cleanup selected account and transactions
            Core.Instance.CurrentAccount = null;
        }
    }
}
