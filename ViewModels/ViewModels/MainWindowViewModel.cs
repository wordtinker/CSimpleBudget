using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;

namespace ViewModels
{
    // TODO LATER Enum extension for budget types
    // TODO LATER localization
    // TODO LATER Move DB to cascade

    /// <summary>
    /// Container for Spending object.
    /// </summary>
    public class BudgetBar
    {
        private Spending spending;
        private CategoryNode category;

        // ctor
        public BudgetBar(Spending spending)
        {
            this.spending = spending;
            this.category = new CategoryNode(spending.Category);
        }

        public string Name { get { return category.FullName; } }
        public int Month { get { return spending.Month; } }
        public string MonthName { get { return DateTimeFormatInfo.CurrentInfo.MonthNames[spending.Month - 1]; } }
        /// <summary>
        /// If overspent occured, spent is planned 
        /// budget.
        /// if not - spent is actual sum of transactions.
        /// </summary>
        public decimal Spent
        {
            get
            {
                return spending.Value - Overspent;
            }
        }
        /// <summary>
        /// If overspent occured, sum to spend is 0m.
        /// If not - to spend is difference between budget and
        /// sum of transactions.
        /// </summary>
        public decimal ToSpend
        {
            get
            {
                return spending.Budget - Spent;
            }
        }
        /// <summary>
        /// If sum of transactions is greater than budget
        /// overspent occurs.
        /// If not - it is 0m.
        /// </summary>
        public decimal Overspent
        {
            get
            {
                return Math.Max(0m, spending.Value - spending.Budget);
            }
        }
    }

    public class MainWindowViewModel : BindableBase
    {
        private IUIMainWindowService windowService;
        private Core core;
        private ICommand showHelp;
        private ICommand showBudgetReport;
        private ICommand showBalanceReport;
        private ICommand showCategoriesReport;
        private ICommand manageAccTypes;
        private ICommand manageAccounts;
        private ICommand manageCategories;
        private ICommand manageBudget;
        private ICommand exit;
        private ICommand createFile;
        private ICommand closeFile;
        private ICommand openFile;
        private string openedFile;

        /// <summary>
        /// Property showing that we have enough
        /// info for reports.
        /// </summary>
        public bool CanShowReport
        {
            get
            {
                bool catExists = (from c in core.Categories
                                  where c.Parent != null
                                  select c).Any();
                return !string.IsNullOrEmpty(OpenedFile) && catExists;
            }
        }

        public string OpenedFile
        {
            get { return openedFile; }
            set
            {
               if (SetProperty(ref openedFile, value))
                {
                    OnPropertyChanged(() => CanShowReport);
                }
            }
        }

        public ICommand ShowHelp
        {
            get
            {
                return showHelp ??
                (showHelp = new DelegateCommand(() =>
                {
                    string info = string.Format("SimpleBudget: {0}",
                        CoreAssembly.Version);
                    windowService.ShowMessage(info);
                }));
            }
        }

        public ICommand ShowBudgetReport
        {
            get
            {
                return showBudgetReport ??
                (showBudgetReport = new DelegateCommand(() =>
                {
                    windowService.ShowBudgetReport();
                }, () =>
                {
                    return CanShowReport;
                }).ObservesProperty(() => CanShowReport));
            }
        }

        public ICommand ShowBalanceReport
        {
            get
            {
                return showBalanceReport ??
                (showBalanceReport = new DelegateCommand(() =>
                {
                    windowService.ShowBalanceReport();
                }, () =>
                {
                    return CanShowReport;
                }).ObservesProperty(() => CanShowReport));
            }
        }

        public ICommand ShowCategoriesReport
        {
            get
            {
                return showCategoriesReport ??
                (showCategoriesReport = new DelegateCommand(() =>
                {
                    windowService.ShowCategoriesReport();
                }, () =>
                {
                    return CanShowReport;
                }).ObservesProperty(() => CanShowReport));
            }
        }


        public ICommand ManageAccTypes
        {
            get
            {
                return manageAccTypes ??
                (manageAccTypes = new DelegateCommand(() =>
                {
                    windowService.ManageAccountTypes();
                }, () =>
                {
                    return !string.IsNullOrEmpty(OpenedFile);
                }).ObservesProperty(() => OpenedFile));
            }
        }

		public ICommand ManageAccounts
        {
            get
            {
                return manageAccounts ??
                (manageAccounts = new DelegateCommand(() =>
                {
                    if (Core.Instance.AccountTypes.Count == 0)
                    {
                        windowService.ShowMessage("Set account types first!");
                    }
                    else
                    {
                        windowService.ManageAccounts();
                    }
                }, () =>
                {
                    return !string.IsNullOrEmpty(OpenedFile);
                }).ObservesProperty(() => OpenedFile));
            }
        }
		
		public ICommand ManageCategories
        {
            get
            {
                return manageCategories ??
                (manageCategories = new DelegateCommand(() =>
                {
                    windowService.ManageCategories();
                }, () =>
                {
                    return !string.IsNullOrEmpty(OpenedFile);
                }).ObservesProperty(() => OpenedFile));
            }
        }
		
		public ICommand ManageBudget
        {
            get
            {
                return manageBudget ??
                (manageBudget = new DelegateCommand(() =>
                {
                    windowService.ManageBudget();
                }, () =>
                {
                    return CanShowReport;
                }).ObservesProperty(() => CanShowReport));
            }
        }

        public ICommand Exit
        {
            get
            {
                return exit ??
                (exit = new DelegateCommand(() =>
                {
                    windowService.Shutdown();
                }));
            }
        }

        public ICommand CreateFile
        {
            get
            {
                return createFile ??
                (createFile = new DelegateCommand(() =>
                {
                    FileReader fileHandler = new SQLiteReader();
                    string fileName = windowService.SaveFileDialog(fileHandler.Extension);
                    if (fileName != null)
                    {
                        // Close the file if we have one opened
                        CloseFile.Execute(null);

                        // Create new file
                        if (fileHandler.InitializeFile(fileName) &&
                            fileHandler.LoadFile(fileName))
                        {
                            SaveLastOpenedFile(fileName);
                            if (!core.InitializeNewFileReader(fileHandler))
                            {
                                windowService.ShowMessage("File is corrupted.");
                                CloseFile.Execute(null);
                            }
                        }
                        else
                        {
                            windowService.ShowMessage("Can't create file.");
                        }
                    }
                }));
            }
        }

        public ICommand OpenFile
        {
            get
            {
                return openFile ??
                (openFile = new DelegateCommand(() =>
                {
                    FileReader fileHandler = new SQLiteReader();
                    string fileName = windowService.OpenFileDialog(fileHandler.Extension);
                    if (fileName != null)
                    {
                        if (fileHandler.LoadFile(fileName))
                        {
                            SaveLastOpenedFile(fileName);
                            if (!core.InitializeNewFileReader(fileHandler))
                            {
                                windowService.ShowMessage("File is corrupted.");
                                CloseFile.Execute(null);
                            }
                        }
                        else
                        {
                            windowService.ShowMessage("Can't open file.");
                        }
                    }
                }));
            }
        }

        public ICommand CloseFile
        {
            get
            {
                return closeFile ??
                (closeFile = new DelegateCommand(() =>
                {
                    SaveLastOpenedFile(string.Empty);
                    core.InitializeNewFileReader(null);
                }, () =>
                {
                    return !string.IsNullOrEmpty(OpenedFile);
                }).ObservesProperty(() => OpenedFile));
            }
        }

        public void ShowTransactionRoll(AccountItem item)
        {
            if (item.Aggregated == false)
            {
                if (!CanShowReport)
                {
                    windowService.ShowMessage("Set categories first!");
                }
                else
                {
                    windowService.ShowTransactionRoll(item);
                }
            }
        }

        public IEnumerable<AccountItem> Accounts
        {
            get
            {
                List<AccountItem> accs = 
                    (from acc in core.Accounts
                     where acc.Closed == false
                     select new AccountItem(acc)).ToList();
                if (accs.Count > 0)
                {
                    AccountItem totalAccItem = new AccountItem(
                        new Account
                        {
                            Name = "Total",
                            Balance = accs.Sum(acc => acc.Balance)
                        });
                    totalAccItem.Aggregated = true;
                    accs.Add(totalAccItem);
                }
                return accs;
            }
        }

        public IEnumerable<BudgetBar> Bars
        {
            get
            {
                return from spending in core.CurrentMonthSpendings
                       select new BudgetBar(spending);
            }
        }

        public void LoadLastOpenedFile()
        {
            string fileName = windowService.LastSavedFileName;
            if (fileName != string.Empty)
            {
                FileReader fileHandler = new SQLiteReader();
                if (fileHandler.LoadFile(fileName))
                {
                    if (!core.InitializeNewFileReader(fileHandler))
                    {
                        windowService.ShowMessage("File is corrupted.");
                        CloseFile.Execute(null);
                    }
                    OpenedFile = fileName;
                }
            }
        }

        public void SaveLastOpenedFile(string fileName)
        {
            OpenedFile = fileName;
            windowService.LastSavedFileName = fileName;
        }

        // ctor
        public MainWindowViewModel(IUIMainWindowService windowService)
        {
            this.windowService = windowService;
            core = Core.Instance;
            core.Accounts.ListChanged += (sender, e) =>
            {
                OnPropertyChanged(() => Accounts);
            };
            core.Categories.CollectionChanged += (sender, r) =>
            {
                OnPropertyChanged(() => CanShowReport);
            };
            core.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "CurrentMonthSpendings")
                {
                    OnPropertyChanged(() => Bars);
                }
            };
            LoadLastOpenedFile();
        }
    }
}
