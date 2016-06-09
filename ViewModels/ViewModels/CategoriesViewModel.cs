using Prism.Mvvm;
using System.Collections.ObjectModel;
using Models;
using System.Collections.Generic;

namespace ViewModels
{
    public class Node
    {
        public string Title { get; set; }
        public ObservableCollection<Node> Items { get; set; }
        public Node Parent{ get; set; }

        public Node()
        {
            this.Items = new ObservableCollection<Node>();
        }
    }

    public class CategoriesViewModel : BindableBase
    {
        public ObservableCollection<Node> Categories { get; }
        public ObservableCollection<string> TopCategories{ get; }

        public bool DeleteCategory(Node category)
        {
            if (Core.Instance.DeleteCategory(category.Title, category.Parent?.Title ?? string.Empty))
            {
                if(category.Parent == null)
                {
                    TopCategories.Remove(category.Title);
                    Categories.Remove(category);
                }
                else
                {
                    category.Parent.Items.Remove(category);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddCategory(string name, string parent)
        {
            if(Core.Instance.AddCategory(name, parent))
            {
                if(parent == string.Empty)
                {
                    TopCategories.Insert(TopCategories.Count - 1, name);
                    Categories.Add(new Node { Title = name });
                } else
                {
                    foreach (Node cat in Categories)
                    {
                        if (cat.Title == parent)
                        {
                            cat.Items.Add(new Node { Title = name, Parent = cat });
                            break;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public CategoriesViewModel()
        {
            List<string> parents = Core.Instance.GetTopCategories();
            TopCategories = new ObservableCollection<string>(parents);
            TopCategories.Add(string.Empty);

            Categories = new ObservableCollection<Node>();
            foreach (string parent in parents)
            {
                Node top = new Node { Title = parent };
                Categories.Add(top);
                foreach (string child in Core.Instance.GetSubcategories(parent))
                {
                    top.Items.Add(new Node { Title = child, Parent = top });
                }
            }
        }
    }
}
