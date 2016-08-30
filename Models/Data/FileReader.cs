using System.Collections.Generic;

namespace Models
{
    public abstract class FileReader
    {
        public abstract string Extension { get; }

        public abstract bool InitializeFile(string fileName);
        public abstract bool LoadFile(string fileName);

        public abstract List<Account> SelectAccounts();
        public abstract bool AddAccount(string name);
        public abstract void UpdateAccount(Account acc);

        public abstract List<Category> SelectCategories();
        public abstract bool AddCategory(string name, string parent);
        public abstract bool DeleteCategory(Category cat);
    }
}
