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
                       where c.Parent == null && c.Name != string.Empty
                       select new Node(c);
            }
        }
        public IEnumerable<string> TopCategories{
            get
            {
                return from c in Core.Instance.Categories
                       where c.Parent == null
                       select c.Name;
            }
        }

        public bool DeleteCategory(Node node)
        {
            return Core.Instance.DeleteCategory(node.category);
        }

        public bool AddCategory(string name, string parent)
        {
            return Core.Instance.AddCategory(name, parent);
        }

        public CategoriesViewModel()
        {
            Core.Instance.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                OnPropertyChanged(() => Categories);
                OnPropertyChanged(() => TopCategories);
            };
        }
    }
}
