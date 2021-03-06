﻿using System;
using System.Collections.Generic;

namespace Models
{
    public abstract class FileReader
    {

        public abstract string Extension { get; }

        public abstract bool InitializeFile(string fileName);
        public abstract bool LoadFile(string fileName);
        public abstract void ReleaseFile();

        internal abstract List<string> SelectAccTypes();
        internal abstract bool AddAccType(string name);
        internal abstract bool DeleteAccType(string name);

        internal abstract List<Account> SelectAccounts();
        internal abstract bool AddAccount(string name, string accType, out Account acc);
        internal abstract bool UpdateAccount(Account acc);
        internal abstract bool DeleteAccount(Account acc);

        internal abstract List<Category> SelectCategories();
        internal abstract bool AddCategory(string name, Category parent, out Category cat);
        internal abstract bool DeleteCategory(Category cat);

        internal abstract List<Transaction> SelectTransactions(Account acc);
        internal abstract List<Transaction> SelectTransactions(int selectedYear, int selectedMonth, Category selectedCategory);
        internal abstract List<Transaction> SelectTransactions(int selectedYear, int selectedMonth);
        internal abstract DateTime SelectLastTransactionDate(int month, int year);
        internal abstract decimal SelectTransactionsCombinedUpTo(DateTime date);
        internal abstract decimal SelectTransactionsCombined(int currentYear, int currentMonth, Category cat);
        internal abstract bool DeleteTransaction(Transaction transaction);
        internal abstract bool UpdateTransaction(Transaction tr, DateTime date, decimal amount, string info, Category category);
        internal abstract bool AddTransaction(
            Account currentAccount, DateTime date, decimal amount, string info, Category category, out Transaction newTr);

        internal abstract List<BudgetRecord> SelectRecords(int currentYear, int currentMonth);
        internal abstract decimal SelectRecordsCombined(int currentYear, int currentMonth, Category cat);
        internal abstract bool DeleteRecord(BudgetRecord record);
        internal abstract bool AddRecord(decimal amount, Category category, BudgetType budgetType, int onDay, int selectedMonth, int selectedYear, out BudgetRecord newRecord);
        internal abstract bool UpdateRecord(BudgetRecord record, decimal amount, Category category, BudgetType budgetType, int onDay, int selectedMonth, int selectedYear);

        internal abstract int? GetMaximumYear();
        internal abstract int? GetMinimumYear();
    }
}
