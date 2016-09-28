using Microsoft.Win32;
using System.Windows;
using ViewModels;

namespace SimpleBudget
{
    class MainWindowService : IUIMainWindowService
    {
        private MainWindow mainWindow;

        public MainWindowService(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void ShowBalanceReport()
        {
            // TODO
            MessageBox.Show("TODO balance !");
        }

        public void ShowBudgetReport()
        {
            // TODO
            MessageBox.Show("TODO budget !");
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowTransactionRoll(AccountItem accItem)
        {
            TransactionRoll window = new TransactionRoll();
            TransactionRollService service = new TransactionRollService(window);
            TransactionRollViewModel vm = new TransactionRollViewModel(accItem, service);
            window.DataContext = vm;
            window.Owner = mainWindow;
            window.ShowDialog();
        }
		
        public void ManageAccountTypes()
        {
            AccTypeViewModel vm = new AccTypeViewModel();
            AccTypeManager window = new AccTypeManager();
            window.DataContext = vm;
            window.Owner = mainWindow;
            window.ShowDialog();
        }

		public void ManageAccounts()
		{
            AccountsViewModel vm = new AccountsViewModel();
            AccountsManager window = new AccountsManager();
            window.DataContext = vm;
            window.Owner = mainWindow;
            window.ShowDialog();
        }

        public void ManageCategories()
		{
            CategoriesViewModel vm = new CategoriesViewModel();
            CategoriesManager window = new CategoriesManager();
            window.DataContext = vm;
            window.Owner = mainWindow;
            window.ShowDialog();
		}

		public void ManageBudget()
		{
            BudgetManager window = new BudgetManager();
            BudgetManagerService service = new BudgetManagerService(window);
            BudgetManagerViewModel vm = new BudgetManagerViewModel(service);
            window.DataContext = vm;
            window.Owner = mainWindow;
            window.ShowDialog();
		}

        public void Shutdown()
        {
            App.Current.Shutdown();
        }

        public string SaveFileDialog(string fileExtension)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = fileExtension;
            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            else
            {
                return null;
            }
        }

        public string OpenFileDialog(string fileExtension)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = fileExtension;
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            else
            {
                return null;
            }
        }

        public void SetConfig(string key, string value)
        {
            Config.AddUpdateConfig(key, value);
        }

        public string GetConfig(string key)
        {
            return Config.ReadSetting(key);
        }
    }
}
