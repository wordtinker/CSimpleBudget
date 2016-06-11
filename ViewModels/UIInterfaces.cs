
namespace ViewModels
{
    public interface IUIMainWindowService
    {
        // TODO
        void ShowMessage(string message);
        void ShowBudgetReport();
        void ShowBalanceReport();
		void ManageAccounts();
		void ManageCategories();
		void ManageBudget();
        void Shutdown();
        void SetConfig(string key, string value);
        string GetConfig(string key);
        string SaveFileDialog(string fileExtension);
        string OpenFileDialog(string fileExtension);
    }

    public interface IUIBudgetWindowService
    {
        // TODO
    }
}
