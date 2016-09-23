using System;
using System.Windows;
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
            editor.ShowDialog();
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public bool RequestMonthAndYear(out int month, out int year)
        {
            BudgetManagerCopyRequest requestWindow = new BudgetManagerCopyRequest();
            BudgetManagerCopyRequestViewModel context = new BudgetManagerCopyRequestViewModel();
            requestWindow.DataContext = context;
            requestWindow.Owner = parentWindow;
            if (requestWindow.ShowDialog() == true)
            {
                month = context.SelectedMonth;
                year = context.SelectedYear;
                return true;
            }
            else
            {
                month = 0;
                year = 0;
                return false;
            }
        }
    }
}
