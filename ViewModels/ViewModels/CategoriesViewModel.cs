using Prism.Mvvm;
using System.Collections.ObjectModel;
using Models;
using System.Collections.Generic;
using System.Linq;

namespace ViewModels
{
    public class CategoryNode
    {
        private string separator = "--";
        public Category category;

        public string Title { get { return category.Name; } }
        public string FullName
        {
            get
            {
                return string.Format("{0}{1}{2}", category.Parent.Name, separator, category.Name);
            }
        }
        public ObservableCollection<CategoryNode> Items { get; }

        public CategoryNode(Category cat)
        {
            category = cat;
            Items = new ObservableCollection<CategoryNode>();
            foreach (Category item in cat.Children)
            {
                Items.Add(new CategoryNode(item));
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as CategoryNode;

            if (item == null)
            {
                return false;
            }

            return category.Equals(item.category);
        }
    }

    public class CategoriesViewModel : BindableBase
    {
        public IEnumerable<CategoryNode> Categories {
            get
            {
                return from c in Core.Instance.Categories
                       where c.Parent == null
                       select new CategoryNode(c);
            }
        }

        public bool DeleteCategory(CategoryNode node)
        {
            return Core.Instance.DeleteCategory(node.category);
        }

        public bool AddCategory(string name, CategoryNode parentNode)
        {
            return Core.Instance.AddCategory(name, parentNode?.category);
        }

        public CategoriesViewModel()
        {
            Core.Instance.Categories.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged(() => Categories);
            };
        }
    }
}
