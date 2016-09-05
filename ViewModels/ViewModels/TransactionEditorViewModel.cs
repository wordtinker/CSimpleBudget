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

        public IEnumerable<Node> Categories
        {
            get
            {
                return from c in Core.Instance.Categories
                       where c.Parent != null
                       select new Node(c);
            }
        }
        // TODO set date properly, not updating?
        public DateTime Date { get; set; }
        // TODO only decimal!
        public decimal Amount { get; set; }
        public string Info { get; set; }
        public Node Category { get; set; }

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
                    // TODO close()!
                }));
            }
        }

        public TransactionEditorViewModel()
        {
            Date = DateTime.Now;
            Category = new Node((from c in Core.Instance.Categories where c.Parent != null select c).First());
        }

        public TransactionEditorViewModel(Transaction tr)
        {
            this.tr = tr;
            Date = tr.Date;
            Amount = tr.Amount;
            Info = tr.Info;
            Category = new Node(tr.Category);
        }
    }
}
