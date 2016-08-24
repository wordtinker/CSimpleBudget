using Prism.Mvvm;
using System.Collections.Generic;

namespace Models
{
    public class Core : BindableBase
    {
        private static readonly Core instance = new Core();
        private FileReader storage;

        public static Core Instance
        {
            get
            {
                return instance;
            }
        }

        private List<Category> categories = new List<Category>();
        public List<Category> Categories {
            get
            {
                categories.Clear();
                if (storage != null)
                {
                    foreach (Category cat in storage.SelectCategories())
                    {
                        categories.Add(cat);
                    }
                }
                return categories;
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
                OnPropertyChanged(() => Categories);
            }
        }

        public bool AddCategory(string name, string parent)
        {
            if (storage.AddCategory(name, parent))
            {
                OnPropertyChanged(() => Categories);
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
                OnPropertyChanged(() => Categories);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
