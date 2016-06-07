using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using Models;

namespace ViewModels
{
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

        // TODO menu commands
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
                }));
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
                }));
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
                }));
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
                }));
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
                }));
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
                }));
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
            //LoadLastOpenedFile();
            // TODO 
        }
    }
}
