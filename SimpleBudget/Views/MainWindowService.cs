using Microsoft.Win32;
using System;
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
		
		public void ManageAccounts()
		{
			// TODO
            MessageBox.Show("TODO Acc !");
		}

        public void ManageCategories()
		{
		    // TODO
            MessageBox.Show("TODO cats !");	
		}

		public void ManageBudget()
		{
			// TODO
            MessageBox.Show("TODO budget !");
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

        // TODO
    }
}
