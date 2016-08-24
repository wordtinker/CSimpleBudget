using System.Collections.Generic;

namespace Models
{
    public abstract class FileReader
    {
        public abstract string Extension { get; }

        public abstract bool InitializeFile(string fileName);
        public abstract bool LoadFile(string fileName);

        public abstract List<Category> SelectCategories();
        public abstract bool AddCategory(string name, string parent);
        public abstract bool DeleteCategory(Category cat);
    }
}
