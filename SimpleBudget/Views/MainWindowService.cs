﻿using Microsoft.Win32;
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
            BalanceReport window = new BalanceReport();
            BalanceReportViewModel vm = new BalanceReportViewModel();
            window.DataContext = vm;
            window.Owner = mainWindow;
            window.ShowDialog();
        }

        public void ShowBudgetReport()
        {
            BudgetReport window = new BudgetReport();
            BudgetReportViewModel vm = new BudgetReportViewModel();
            window.DataContext = vm;
            window.Owner = mainWindow;
            window.ShowDialog();
        }

        public void ShowCategoriesReport()
        {
            CategoriesReport window = new CategoriesReport();
            CategoriesReportViewModel vm = new CategoriesReportViewModel();
            window.DataContext = vm;
            window.Owner = mainWindow;
            window.ShowDialog();
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

        public string LastSavedFileName
        {
            get
            {
                // Fetch filename from config file, it could be empty.
                return Properties.Settings.Default.FileName;
            }
            set
            {
                Properties.Settings.Default.FileName = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
