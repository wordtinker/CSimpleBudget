using ViewModels;

namespace SimpleBudget
{
    class BudgetManagerService : IUIBudgetWindowService
    {
        private BudgetManager parentWindow;

        public BudgetManagerService(BudgetManager managerWindow)
        {
            this.parentWindow = managerWindow;
        }

        public void ShowBudgetRecordEditor(BudgetRecordEditorViewModel vm)
        {
            BudgetRecordEditor editor = new BudgetRecordEditor();
            editor.DataContext = vm;
            editor.Owner = parentWindow;
            editor.Show();
        }
    }
}
