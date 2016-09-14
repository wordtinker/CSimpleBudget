using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using Models;
using System.Collections.Generic;
using System.Linq;

namespace ViewModels
{
    public class BudgetBar
    {
        public string Name { get; set; }
        public decimal Spent { get; set; }
        public decimal ToSpend { get; set; }
        public decimal Overspent { get; set; }
        // TODO
    }

    public class MainWindowViewModel : BindableBase
    {
        private IUIMainWindowService windowService;
        private Core core;
        private ICommand showHelp;
        private ICommand showBudgetReport;
        private ICommand showBalanceReport;
        private ICommand manageAccounts;
        private ICommand manageCategories;
        private ICommand manageBudget;
        private ICommand exit;
        private ICommand createFile;
        private ICommand closeFile;
        private ICommand openFile;
        private string openedFile;
        

        public string OpenedFile
        {
            get { return openedFile; }
            set
            {
                SetProperty(ref openedFile, value);
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
                    return !string.IsNullOrEmpty(OpenedFile);
                }).ObservesProperty(() => OpenedFile));
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
                    windowService.ManageAccounts();
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
                    return !string.IsNullOrEmpty(OpenedFile);
                }).ObservesProperty(() => OpenedFile));
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
                        if (fileHandler.InitializeFile(fileName) &&
                            fileHandler.LoadFile(fileName))
                        {
                            SaveLastOpenedFile(fileName);
                            core.Storage = fileHandler;
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
                            core.Storage = fileHandler;
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
                    core.Storage = null;
                }, () =>
                {
                    return !string.IsNullOrEmpty(OpenedFile);
                }).ObservesProperty(() => OpenedFile));
            }
        }

        public void ShowTransactionRoll(AccountItem item)
        {
            core.SelectedAccount = item.account;
            windowService.ShowTransactionRoll();
        }

        // TODO total line
        public IEnumerable<AccountItem> Accounts
        {
            get
            {
                return from acc in core.Accounts
                       where acc.Closed == false
                       select new AccountItem(acc);
            }
        }

        // TODO
        public IEnumerable<BudgetBar> Bars
        {
            get
            {
              var bars = new List<BudgetBar>
                {
                    new BudgetBar { Name="B0/S120", Spent=0, ToSpend=0, Overspent=120},
                    new BudgetBar { Name="B100/S50", Spent=50, ToSpend=50, Overspent=0},
                    new BudgetBar { Name="B100/S0", Spent=0, ToSpend=100, Overspent=0},
                    new BudgetBar { Name="B100/120", Spent=100, ToSpend=0, Overspent=20}
                };
                return bars;
            }
        }

        public void LoadLastOpenedFile()
        {
            string fileName = windowService.GetConfig("LastFile");
            if (fileName != string.Empty)
            {
                FileReader fileHandler = new SQLiteReader();
                if (fileHandler.LoadFile(fileName))
                {
                    core.Storage = fileHandler;
                    OpenedFile = fileName;
                }
            }
        }

        public void SaveLastOpenedFile(string fileName)
        {
            OpenedFile = fileName;
            windowService.SetConfig("LastFile", fileName);
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
            LoadLastOpenedFile();
        }
    }
}
