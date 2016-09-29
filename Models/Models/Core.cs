﻿using Prism.Mvvm;
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
        private int currentYear = DateTime.Now.Year;
        private int currentMonth = DateTime.Now.Month;

        public static Core Instance
        {
            get
            {
                return instance;
            }
        }

        public FileReader Storage
        {
            internal get
            {
                return storage;
            }
            set
            {
                storage = value;

                AccountTypes.Clear();
                Accounts.Clear();
                Categories.Clear();
                if (storage != null)
                {
                    storage.SelectAccTypes().ForEach(AccountTypes.Add);
                    storage.SelectAccounts().ForEach(Accounts.Add);
                    storage.SelectCategories().ForEach(Categories.Add);
                }
                OnPropertyChanged(() => CurrentMonthSpendings);
            }
        }

        public void GetActiveBudgetYears(out int minYear, out int maxYear)
        {
            minYear = Storage.GetMinimumYear() ?? DateTime.Today.Year;
            maxYear = Storage.GetMaximumYear() ?? DateTime.Today.Year;
        }

        public ObservableCollection<string> AccountTypes { get; } = new ObservableCollection<string>();

        public bool AddAccType(string name)
        {
            if (Storage.AddAccType(name))
            {
                AccountTypes.Add(name);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteAccType(string name)
        {
            if (Storage.DeleteAccType(name))
            {
                AccountTypes.Remove(name);
                return true;
            }
            else
            {
                return false;
            }
        }

        public BindingList<Account> Accounts { get; } = new BindingList<Account>();

        public void UpdateAccount(Account acc)
        {
            Storage.UpdateAccount(acc);

            // exbudget on/off could change spending view 
            OnPropertyChanged(() => CurrentMonthSpendings);
        }

        public bool AddAccount(string accName)
        {
            string newAccDefaultType = AccountTypes[0];
            Account newAcc;
            if (Storage.AddAccount(accName, newAccDefaultType, out newAcc))
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
            if (Storage.DeleteAccount(account))
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
            if (Storage.AddCategory(name, parent, out newCat))
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
            if (Storage.DeleteCategory(cat))
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

        public List<Transaction> GetTransactions(Account selectedAccount)
        {
            return Storage.SelectTransactions(selectedAccount);
        }

        public bool DeleteTransaction(Transaction transaction)
        {
            if (Storage.DeleteTransaction(transaction))
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

        public bool AddTransaction(
            Account acc, DateTime date, decimal amount, string info, Category category, out Transaction newTransaction)
        {
            if (Storage.AddTransaction(acc, date, amount, info, category, out newTransaction))
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

        public void UpdateTransaction(Transaction tr, DateTime date, decimal amount, string info, Category category)
        {
            bool updateFlag = (tr.Date.Year == currentYear && tr.Date.Month == currentMonth) ||
                              (date.Year == currentYear && date.Month == currentMonth);
            if (Storage.UpdateTransaction(tr, date, amount, info, category))
            {
                if (updateFlag)
                {
                    // Spending table has changed
                    OnPropertyChanged(() => CurrentMonthSpendings);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedYear"></param>
        /// <param name="selectedMonth">1-based</param>
        /// <returns></returns>
        public List<BudgetRecord> GetRecords(int selectedYear, int selectedMonth)
        {
            return Storage.SelectRecords(selectedYear, selectedMonth);
        }

        public void CopyRecords(int fromMonth, int fromYear, int toMonth, int toYear)
        {
            Storage.SelectRecords(fromYear, fromMonth).ForEach((r) =>
            {
                BudgetRecord _;
                AddRecord(r.Amount, r.Category, r.Type, r.OnDay,
                    toMonth, toYear, out _);
            });
        }

        public bool DeleteRecord(BudgetRecord record)
        {
            if (Storage.DeleteRecord(record))
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

        public bool AddRecord(
            decimal amount, Category category, BudgetType budgetType,
            int onDay, int selectedMonth, int selectedYear,
            out BudgetRecord newRecord)
        {
            if (Storage.AddRecord(
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

        public bool UpdateRecord(BudgetRecord record,
            decimal amount, Category category, BudgetType budgetType,
            int onDay, int selectedMonth, int selectedYear)
        {
            bool updateFlag = (record.Month == currentMonth && record.Year == currentYear) ||
                              (selectedMonth == currentMonth && selectedYear == currentYear);
            if (Storage.UpdateRecord(record, amount, category, budgetType, onDay, selectedMonth, selectedYear))
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

        public List<Spending> CurrentMonthSpendings
        {
            get
            {
                List<Spending> spendings = new List<Spending>();

                var subcats = from c in Categories
                              where c.Parent != null
                              select c;

                foreach (Category cat in subcats)
                {
                    decimal budget = Math.Abs(Storage.SelectRecordsCombined(currentYear, currentMonth, cat));
                    decimal spent = Math.Abs(Storage.SelectTransactionsCombined(currentYear, currentMonth, cat));
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
