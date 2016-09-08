
namespace ViewModels
{
    public interface IUIMainWindowService
    {
        void ShowMessage(string message);
        void ShowBudgetReport();
        void ShowBalanceReport();
        void ShowTransactionRoll();
		void ManageAccounts();
		void ManageCategories();
		void ManageBudget();
        void Shutdown();
        void SetConfig(string key, string value);
        string GetConfig(string key);
        string SaveFileDialog(string fileExtension);
        string OpenFileDialog(string fileExtension);
    }

    public interface IUITransactionRollService
    {
        void ShowTransactionEditor(TransactionEditorViewModel vm);
    }

    public interface IUIBudgetWindowService
    {
        void ShowBudgetRecordEditor(BudgetRecordEditorViewModel vm);
    }
}
