using System;

namespace ViewModels
{
    public interface IUIMainWindowService
    {
        void ShowMessage(string message);
        void ShowBudgetReport();
        void ShowBalanceReport();
        void ShowTransactionRoll(AccountItem accItem);
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
        bool ShowTransactionEditor(
            TransactionEditorViewModel vm, out DateTime date, out decimal amount, out string info, out CategoryNode catNode);
    }

    public interface IUIBudgetWindowService
    {
        bool ShowBudgetRecordEditor(BudgetRecordEditorViewModel vm, out RecordItem newRecordItem);
        void ShowMessage(string message);
        bool RequestMonthAndYear(out int monthToCopyFrom, out int yearToCopyFrom);
    }
}
