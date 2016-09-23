using System;
using ViewModels;

namespace SimpleBudget
{
    class TransactionRollService : IUITransactionRollService
    {
        private TransactionRoll rollWindow;

        public TransactionRollService(TransactionRoll rollWindow)
        {
            this.rollWindow = rollWindow;
        }

        public bool ShowTransactionEditor(
            TransactionEditorViewModel vm, out DateTime date, out decimal amount, out string info, out CategoryNode catNode)
        {
            TransactionEditor window = new TransactionEditor();
            window.DataContext = vm;
            window.Owner = rollWindow;
            if (window.ShowDialog() == true)
            {
                date = vm.Date;
                amount = vm.Amount;
                info = vm.Info;
                catNode = vm.Category;
                return true;
            }
            else
            {
                date = DateTime.Now;
                amount = 0m;
                info = null;
                catNode = null;
                return false;
            }
        }
    }
}
