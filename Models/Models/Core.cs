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
        // Singletone instance of the core
        private static readonly Core instance = new Core();
        private FileReader storage;
        private int currentYear = DateTime.Now.Year;
        private int currentMonth = DateTime.Now.Month;

        public static Core Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Connects to the new instance of fileReader
        /// and (re)initializes all data of the programm.
        /// </summary>
        /// <param name="fileReader"></param>
        /// <returns></returns>
        public bool InitializeNewFileReader(FileReader fileReader)
        {
            // Release previous storage
            storage?.ReleaseFile();

            // Get data from new storage
            storage = fileReader;
            AccountTypes.Clear();
            Accounts.Clear();
            Categories.Clear();
            if (storage != null)
            {
                try
                {
                    storage.SelectAccTypes().ForEach(AccountTypes.Add);
                    storage.SelectAccounts().ForEach(Accounts.Add);
                    storage.SelectCategories().ForEach(Categories.Add);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            OnPropertyChanged(() => CurrentMonthSpendings);
            return true;
        }

        /// <summary>
        /// Provides a pair of years during which a budget records were set.
        /// Default is current year.
        /// </summary>
        /// <param name="minYear"></param>
        /// <param name="maxYear"></param>
        public void GetActiveBudgetYears(out int minYear, out int maxYear)
        {
            minYear = storage.GetMinimumYear() ?? DateTime.Today.Year;
            maxYear = storage.GetMaximumYear() ?? DateTime.Today.Year;
        }

        /// <summary>
        /// Collection of account types.
        /// </summary>
        public ObservableCollection<string> AccountTypes { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Adds new account type to acc type collection.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddAccType(string name)
        {
            if (storage.AddAccType(name))
            {
                AccountTypes.Add(name);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes account type from acc type collection.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DeleteAccType(string name)
        {
            if (storage.DeleteAccType(name))
            {
                AccountTypes.Remove(name);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Collection of accounts.
        /// </summary>
        public BindingList<Account> Accounts { get; } = new BindingList<Account>();

        /// <summary>
        /// Updates properties of the account.
        /// </summary>
        /// <param name="acc"></param>
        public void UpdateAccount(Account acc)
        {
            storage.UpdateAccount(acc);

            // exbudget on/off could change spending view 
            OnPropertyChanged(() => CurrentMonthSpendings);
        }

        /// <summary>
        /// Adds account to account list.
        /// </summary>
        /// <param name="accName"></param>
        /// <returns></returns>
        public bool AddAccount(string accName)
        {
            string newAccDefaultType = AccountTypes[0];
            Account newAcc;
            if (storage.AddAccount(accName, newAccDefaultType, out newAcc))
            {
                Accounts.Add(newAcc);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes account from account list.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Collection of categories.
        /// </summary>
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        /// <summary>
        /// Adds category to the collection of categories.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes category from the collection of categories.
        /// </summary>
        /// <param name="cat"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Provides list of transactions for a given account.
        /// </summary>
        /// <param name="selectedAccount"></param>
        /// <returns></returns>
        public List<Transaction> GetTransactions(Account selectedAccount)
        {
            return storage.SelectTransactions(selectedAccount);
        }

        /// <summary>
        /// Provides list of transactions for a given year,
        /// month and category.
        /// </summary>
        /// <param name="selectedYear"></param>
        /// <param name="selectedMonth"></param>
        /// <param name="selectedCategory"></param>
        /// <returns></returns>
        public List<Transaction> GetTransactions(int selectedYear, int selectedMonth, Category selectedCategory)
        {
            return storage.SelectTransactions(selectedYear, selectedMonth, selectedCategory);
        }

        /// <summary>
        /// Deletes transaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool DeleteTransaction(Transaction transaction)
        {
            if (storage.DeleteTransaction(transaction))
            {
                if (transaction.Date.Year == currentYear && transaction.Date.Month == currentMonth)
                {
                    // Spending table has changed
                    OnPropertyChanged(() => CurrentMonthSpendings);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds transaction.
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="date"></param>
        /// <param name="amount"></param>
        /// <param name="info"></param>
        /// <param name="category"></param>
        /// <param name="newTransaction"></param>
        /// <returns></returns>
        public bool AddTransaction(
            Account acc, DateTime date, decimal amount, string info, Category category, out Transaction newTransaction)
        {
            if (storage.AddTransaction(acc, date, amount, info, category, out newTransaction))
            {
                if (newTransaction.Date.Year == currentYear && newTransaction.Date.Month == currentMonth)
                {
                    // Spending table has changed
                    OnPropertyChanged(() => CurrentMonthSpendings);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Updates properties of the transaction.
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="date"></param>
        /// <param name="amount"></param>
        /// <param name="info"></param>
        /// <param name="category"></param>
        public void UpdateTransaction(Transaction tr, DateTime date, decimal amount, string info, Category category)
        {
            bool updateFlag = (tr.Date.Year == currentYear && tr.Date.Month == currentMonth) ||
                              (date.Year == currentYear && date.Month == currentMonth);
            if (storage.UpdateTransaction(tr, date, amount, info, category))
            {
                if (updateFlag)
                {
                    // Spending table has changed
                    OnPropertyChanged(() => CurrentMonthSpendings);
                }
            }
        }

        /// <summary>
        /// Provides list of budget records for a given year and month.
        /// </summary>
        /// <param name="selectedYear"></param>
        /// <param name="selectedMonth">1-based</param>
        /// <returns></returns>
        public List<BudgetRecord> GetRecords(int selectedYear, int selectedMonth)
        {
            return storage.SelectRecords(selectedYear, selectedMonth);
        }

        /// <summary>
        /// Copy budget records from one month to another.
        /// </summary>
        /// <param name="fromMonth"></param>
        /// <param name="fromYear"></param>
        /// <param name="toMonth"></param>
        /// <param name="toYear"></param>
        public void CopyRecords(int fromMonth, int fromYear, int toMonth, int toYear)
        {
            storage.SelectRecords(fromYear, fromMonth).ForEach((r) =>
            {
                BudgetRecord _;
                AddRecord(r.Amount, r.Category, r.Type, r.OnDay,
                    toMonth, toYear, out _);
            });
        }

        /// <summary>
        /// Deletes budget record.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool DeleteRecord(BudgetRecord record)
        {
            if (storage.DeleteRecord(record))
            {
                if (record.Month == currentMonth && record.Year == currentYear)
                {
                    // Spending table has changed
                    OnPropertyChanged(() => CurrentMonthSpendings);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds budget record.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="category"></param>
        /// <param name="budgetType"></param>
        /// <param name="onDay"></param>
        /// <param name="selectedMonth"></param>
        /// <param name="selectedYear"></param>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        public bool AddRecord(
            decimal amount, Category category, BudgetType budgetType,
            int onDay, int selectedMonth, int selectedYear,
            out BudgetRecord newRecord)
        {
            if (storage.AddRecord(
                amount, category, budgetType, onDay, selectedMonth, selectedYear, out newRecord))
            {
                if (newRecord.Month == currentMonth && newRecord.Year == currentYear)
                {
                    // Spending table has changed
                    OnPropertyChanged(() => CurrentMonthSpendings);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Updates properties of the budget record.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="amount"></param>
        /// <param name="category"></param>
        /// <param name="budgetType"></param>
        /// <param name="onDay"></param>
        /// <param name="selectedMonth"></param>
        /// <param name="selectedYear"></param>
        /// <returns></returns>
        public bool UpdateRecord(BudgetRecord record,
            decimal amount, Category category, BudgetType budgetType,
            int onDay, int selectedMonth, int selectedYear)
        {
            bool updateFlag = (record.Month == currentMonth && record.Year == currentYear) ||
                              (selectedMonth == currentMonth && selectedYear == currentYear);
            if (storage.UpdateRecord(record, amount, category, budgetType, onDay, selectedMonth, selectedYear))
            {
                if (updateFlag)
                {
                    // Spending table has changed
                    OnPropertyChanged(() => CurrentMonthSpendings);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Property that holds the list of spendings for current month.
        /// </summary>
        public List<Spending> CurrentMonthSpendings
        {
            get
            {
                return GetSpendings(currentYear, currentMonth);
            }
        }

        /// <summary>
        /// Selects spendings for a given month and year.
        /// </summary>
        /// <param name="selectedYear"></param>
        /// <param name="selectedMonth"></param>
        /// <returns></returns>
        public List<Spending> GetSpendings(int selectedYear, int selectedMonth)
        {
            List<Spending> spendings = new List<Spending>();

            var subcats = from c in Categories
                          where c.Parent != null
                          select c;

            foreach (Category cat in subcats)
            {
                // should be positive
                decimal budget = Math.Abs(storage.SelectRecordsCombined(selectedYear, selectedMonth, cat));
                decimal spent = Math.Abs(storage.SelectTransactionsCombined(selectedYear, selectedMonth, cat));

                if (budget == 0m && spent == 0m)
                {
                    continue;
                }
                spendings.Add(new Spending
                {
                    Category = cat,
                    Budget = budget,
                    Value = spent,
                    Month = selectedMonth
                });
            }
            return spendings;
        }

        /// <summary>
        /// Provides date of the last account transaction prior to the specified
        /// month and year and overall account balance after that transaction.
        /// </summary>
        /// <param name="selectedMonth"></param>
        /// <param name="selectedYear"></param>
        /// <param name="lastTransactionDate"></param>
        /// <returns></returns>
        public decimal GetBalanceToDate(int month, int year, out DateTime lastTransactionDate)
        {
            lastTransactionDate = storage.SelectLastTransactionDate(month, year);
            return storage.SelectTransactionsCombinedUpTo(lastTransactionDate.AddDays(1));
        }

        // Private constructor of the singletone.
        private Core() { /* Empty */ }
    }
}
