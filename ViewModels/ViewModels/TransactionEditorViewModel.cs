using Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ViewModels
{
    public class TransactionEditorViewModel
    {
        private Transaction tr;
        private ICommand updateTransaction;

        public IEnumerable<CategoryNode> Categories
        {
            get
            {
                return from c in Core.Instance.Categories
                       where c.Parent != null
                       select new CategoryNode(c);
            }
        }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Info { get; set; }
        public CategoryNode Category { get; set; }

        public ICommand UpdateTransaction
        {
            get
            {
                return updateTransaction ??
                (updateTransaction = new DelegateCommand(() =>
                {
                    if (tr == null)
                    {
                        Core.Instance.AddTransaction(Date, Amount, Info, Category.category);
                    }
                    else
                    {
                        Core.Instance.UpdateTransaction(tr, Date, Amount, Info, Category.category);
                    }
                }));
            }
        }

        public TransactionEditorViewModel()
        {
            Date = DateTime.Now;
            Category = new CategoryNode((from c in Core.Instance.Categories where c.Parent != null select c).First());
        }

        public TransactionEditorViewModel(Transaction tr)
        {
            this.tr = tr;
            Date = tr.Date;
            Amount = tr.Amount;
            Info = tr.Info;
            Category = new CategoryNode(tr.Category);
        }
    }
}
