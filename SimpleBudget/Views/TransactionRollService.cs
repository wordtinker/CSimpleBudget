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

        public void ShowTransactionEditor(TransactionEditorViewModel vm)
        {
            TransactionEditor window = new TransactionEditor();
            window.DataContext = vm;
            window.Owner = rollWindow;
            window.Show();
        }
    }
}
