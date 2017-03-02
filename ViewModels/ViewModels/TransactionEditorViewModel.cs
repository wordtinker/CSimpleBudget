using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModels
{
    public class TransactionEditorViewModel
    {
        public IEnumerable<CategoryNode> Categories
        {
            get
            {
                return from c in Core.Instance.Categories
                       where c.Parent != null
                       orderby c.Parent.Name, c.Name
                       select new CategoryNode(c);
            }
        }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Info { get; set; }
        public CategoryNode Category { get; set; }

        public TransactionEditorViewModel()
        {
            Date = DateTime.Now;
            Category = new CategoryNode((from c in Core.Instance.Categories where c.Parent != null select c).First());
        }

        public TransactionEditorViewModel(Transaction tr)
        {
            Date = tr.Date;
            Amount = tr.Amount;
            Info = tr.Info;
            Category = new CategoryNode(tr.Category);
        }
    }
}
