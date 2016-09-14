using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using System.Linq;

namespace Models
{
    public class Core : BindableBase
    {
        private static readonly Core instance = new Core();
        private FileReader storage;
        private Account selectedAccount;
        private int? selectedYear;
        private int? selectedMonth; // 1-based

        public static Core Instance
        {
            get
            {
                return instance;
            }
        }

        public FileReader Storage
        {
            get
            {
                return storage;
            }
            set
            {
                storage = value;

                Accounts.Clear();
                Categories.Clear();
                if (storage != null)
                {
                    storage.SelectAccounts().ForEach(Accounts.Add);
                    storage.SelectCategories().ForEach(Categories.Add);
                }
            }
        }

        public IEnumerable<int> GetActiveYears()
        {
            // TODO min-3 from DB
            int minYear = 2014;
            // TODO max+3 from DB
            int maxYear = DateTime.Now.AddYears(3).Year;
            return Enumerable.Range(minYear, maxYear - minYear);
        }

        // TODO Stub
        // ADD GUI for adding, deleting, and retrieving Acc Types
        public List<string> AccountTypes { get; } = new List<string> { "Bank", "Cash", "Credit Card" };

        public BindingList<Account> Accounts { get; } = new BindingList<Account>();

        public void UpdateAccount(Account acc)
        {
            Storage.UpdateAccount(acc);
        }

        public bool AddAccount(string accName, string accType)
        {
            Account newAcc;
            if (storage.AddAccount(accName, accType, out newAcc))
            {
                Accounts.Add(newAcc);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteAccount(Account account)
        {
            if (storage.DeleteAccount(account))
            {
                Accounts.Remove(account);
                return true;
            }
            else
            {
                return false;
            }
        }

        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        public bool AddCategory(string name, Category parent)
        {
            Category newCat;
            if (storage.AddCategory(name, parent, out newCat))
            {
                Categories.Add(newCat);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteCategory(Category cat)
        {
            if (storage.DeleteCategory(cat))
            {
                cat.Parent?.Children.Remove(cat);
                Categories.Remove(cat);
                return true;
            }
            else
            {
                return false;
            }
        }

        public BindingList<Transaction> Transactions { get; } = new BindingList<Transaction>();
        public Account SelectedAccount
        {
            get
            {
                return selectedAccount;
            }
            set
            {
                selectedAccount = value;
                Transactions.Clear();
                if (selectedAccount != null)
                {
                    storage.SelectTransactions(selectedAccount).ForEach(Transactions.Add);
                }
            }
        }

        public bool DeleteTransaction(Transaction transaction)
        {
            if (storage.DeleteTransaction(transaction))
            {
                Transactions.Remove(transaction);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddTransaction(DateTime date, decimal amount, string info, Category category)
        {
            Transaction newTr;
            if (storage.AddTransaction(SelectedAccount, date, amount, info, category, out newTr))
            {
                Transactions.Add(newTr);
            }
        }


        public void UpdateTransaction(Transaction tr, DateTime date, decimal amount, string info, Category category)
        {
            storage.UpdateTransaction(tr, date, amount, info, category);
        }

        public int? SelectedYear
        {
            get
            {
                return selectedYear;
            }
            set
            {
                if (SetProperty(ref selectedYear, value))
                {
                    Records.Clear();
                    if (SelectedYear != null && SelectedMonth != null)
                    {
                        storage.SelectRecords(SelectedYear.Value, SelectedMonth.Value).ForEach(Records.Add);
                    }
                }
            }
        }
        public int? SelectedMonth
        {
            get
            {
                return selectedMonth;
            }
            set
            {
                if (SetProperty(ref selectedMonth, value))
                {
                    Records.Clear();
                    if (SelectedYear != null && SelectedMonth != null)
                    {
                        storage.SelectRecords(SelectedYear.Value, SelectedMonth.Value).ForEach(Records.Add);
                    }
                }
            }
        }
        public BindingList<BudgetRecord> Records { get; } = new BindingList<BudgetRecord>();

        public bool DeleteRecord(BudgetRecord record)
        {
            if (storage.DeleteRecord(record))
            {
                Records.Remove(record);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddRecord(decimal amount, Category category, BudgetType budgetType, int onDay, int selectedMonth, int selectedYear)
        {
            BudgetRecord newRecord;
            if (storage.AddRecord(
                amount, category, budgetType, onDay, selectedMonth, selectedYear, out newRecord))
            {
                if (SelectedYear == newRecord.Year && SelectedMonth == newRecord.Month)
                {
                    Records.Add(newRecord);
                }
            }
        }

        public void UpdateRecord(BudgetRecord record, decimal amount, Category category, BudgetType budgetType, int onDay, int selectedMonth, int selectedYear)
        {
            storage.UpdateRecord(record, amount, category, budgetType, onDay, selectedMonth, selectedYear);
            if (SelectedYear != record.Year || SelectedMonth != record.Month)
            {
                Records.Remove(record);
            }
        }

        public List<Spending> CurrentMonthSpendings
        {
            get
            {
                // TODO stub
                int currentYear = DateTime.Today.Year;
                int currentMonth = DateTime.Today.Month;

                List<Spending> spendings = new List<Spending>();

                spendings.Add(new Spending
                {
                    Category = new Category { Name="B0", Parent= new Category { Name="S120"} },
                    Budget = 0m,
                    Value = 120m });
                spendings.Add(new Spending
                {
                    Category = new Category { Name = "B100", Parent = new Category { Name = "S50" } },
                    Budget = 100m,
                    Value = 50m
                });
                spendings.Add(new Spending
                {
                    Category = new Category { Name = "B100", Parent = new Category { Name = "S0" } },
                    Budget = 100m,
                    Value = 0m
                });
                spendings.Add(new Spending
                {
                    Category = new Category { Name = "B100", Parent = new Category { Name = "S120" } },
                    Budget = 100m,
                    Value = 120m
                });

                return spendings;
            }
        }

        private Core() { /* Empty */ }

    }
}
