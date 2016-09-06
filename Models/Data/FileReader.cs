﻿using System;
using System.Collections.Generic;

namespace Models
{
    public abstract class FileReader
    {

        public abstract string Extension { get; }

        public abstract bool InitializeFile(string fileName);
        public abstract bool LoadFile(string fileName);

        internal abstract List<Account> SelectAccounts();
        internal abstract bool AddAccount(string name, string accType, out Account acc);
        internal abstract bool UpdateAccount(Account acc);
        internal abstract bool DeleteAccount(Account acc);

        internal abstract List<Category> SelectCategories();
        internal abstract bool AddCategory(string name, Category parent, out Category cat);
        internal abstract bool DeleteCategory(Category cat);

        internal abstract List<Transaction> SelectTransactions(Account acc);
        internal abstract bool DeleteTransaction(Transaction transaction);
        internal abstract bool UpdateTransaction(Transaction tr, DateTime date, decimal amount, string info, Category category);
        internal abstract bool AddTransaction(
            Account currentAccount, DateTime date, decimal amount, string info, Category category, out Transaction newTr);
        
    }
}
