using System.Collections.Generic;

namespace Models
{
    public abstract class FileReader
    {
        public abstract string Extension { get; }

        public abstract bool InitializeFile(string fileName);
        public abstract bool LoadFile(string fileName);
        public abstract List<string> SelectCategories();
        public abstract List<string> SelectSubcategoriesFor(string parent);
        public abstract bool AddCategory(string name);
        public abstract bool AddSubcategory(string name, string parent);
        public abstract bool DeleteCategory(string name);
        public abstract bool DeleteSubcategory(string name, string parent);
    }
}
