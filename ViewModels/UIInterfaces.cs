namespace ViewModels
{
    public interface IUIMainWindowService
    {
        void ShowMessage(string message);
        void ShowBudgetReport();
        void ShowBalanceReport();
        void ShowCategoriesReport();
        void ShowTransactionRoll(AccountItem accItem);
        void ManageAccountTypes();
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
        bool? ShowTransactionEditor(TransactionEditorViewModel vm);
    }

    public interface IUIBudgetWindowService
    {
        bool? ShowBudgetRecordEditor(BudgetRecordEditorViewModel vm);
        void ShowMessage(string message);
        bool RequestMonthAndYear(out int monthToCopyFrom, out int yearToCopyFrom);
    }
}
