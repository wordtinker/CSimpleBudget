﻿using Prism.Commands;
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
            LoadLastOpenedFile();
        }
    }
}