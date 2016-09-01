using System.Collections.Generic;

namespace Models
{
    public abstract class FileReader
    {
        public abstract string Extension { get; }

        public abstract bool InitializeFile(string fileName);
        public abstract bool LoadFile(string fileName);

        public abstract List<Account> SelectAccounts();
        public abstract bool AddAccount(string name, out Account acc);
        public abstract void UpdateAccount(Account acc);
        public abstract bool DeleteAccount(Account acc);

        public abstract List<Category> SelectCategories();
        public abstract bool AddCategory(string name, string parent, out Category cat);
        public abstract bool DeleteCategory(Category cat);

        public abstract List<Transaction> SelectTransactions(Account acc);
    }
}
