using Prism.Mvvm;
using System.Collections.ObjectModel;
using Models;
using System.Collections.Generic;
using System.Linq;

namespace ViewModels
{
    public class Node
    {
        public Category category;

        public string Title { get { return category.Name; } }
        public ObservableCollection<Node> Items { get; }

        public Node(Category cat)
        {
            category = cat;
            Items = new ObservableCollection<Node>();
            foreach (Category item in cat.Children)
            {
                Items.Add(new Node(item));
            }
        }
    }

    public class CategoriesViewModel : BindableBase
    {
        public IEnumerable<Node> Categories {
            get
            {
                return from c in Core.Instance.Categories
                       where c.Parent == null
                       select new Node(c);
            }
        }

        public bool DeleteCategory(Node node)
        {
            return Core.Instance.DeleteCategory(node.category);
        }

        public bool AddCategory(string name, Node parentNode)
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
