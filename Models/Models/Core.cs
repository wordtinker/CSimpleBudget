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

        public void GetActiveBudgetYears(out int minYear, out int maxYear)
        {
            minYear = storage.GetMinimumYear();
            maxYear = storage.GetMaximumYear();
        }

        // TODO Stub
        // ADD GUI for adding, deleting, and retrieving Acc Types
        public List<string> AccountTypes { get; } = new List<string> { "Bank", "Cash", "Credit Card" };

        public BindingList<Account> Accounts { get; } = new BindingList<Account>();

        public void UpdateAccount(Account acc)
        {
            Storage.UpdateAccount(acc);

            // exbudget on/off could change spending view 
            OnPropertyChanged(() => CurrentMonthSpendings);
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
                // Changes the spending view
                OnPropertyChanged(() => CurrentMonthSpendings);
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
                // Changes the spending view
                OnPropertyChanged(() => CurrentMonthSpendings);
            }
        }


        public void UpdateTransaction(Transaction tr, DateTime date, decimal amount, string info, Category category)
        {
            storage.UpdateTransaction(tr, date, amount, info, category);
            // Might change the spending view
            OnPropertyChanged(() => CurrentMonthSpendings);
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
                // Changes the spending view
                OnPropertyChanged(() => CurrentMonthSpendings);
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
                // Changes the spending view
                OnPropertyChanged(() => CurrentMonthSpendings);
            }
        }

        public void UpdateRecord(BudgetRecord record, decimal amount, Category category, BudgetType budgetType, int onDay, int selectedMonth, int selectedYear)
        {
            storage.UpdateRecord(record, amount, category, budgetType, onDay, selectedMonth, selectedYear);
            if (SelectedYear != record.Year || SelectedMonth != record.Month)
            {
                Records.Remove(record);
            }
            // Might change the spending view
            OnPropertyChanged(() => CurrentMonthSpendings);
        }

        public List<Spending> CurrentMonthSpendings
        {
            get
            {
                int currentYear = DateTime.Today.Year;
                int currentMonth = DateTime.Today.Month;

                List<Spending> spendings = new List<Spending>();

                var subcats = from c in Categories
                              where c.Parent != null
                              select c;

                foreach (Category cat in subcats)
                {
                    decimal budget = Math.Abs(storage.SelectRecordsCombined(currentYear, currentMonth, cat));
                    decimal spent = Math.Abs(storage.SelectTransactionsCombined(currentYear, currentMonth, cat));
                    if (budget == 0m && spent == 0m)
                    {
                        continue;
                    }
                    spendings.Add(new Spending
                    {
                        Category = cat,
                        Budget = budget,
                        Value = spent
                    });
                }
                return spendings;
            }
        }

        private Core() { /* Empty */ }

    }
}
