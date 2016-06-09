using System.Windows;
using ViewModels;

namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for CategoriesManager.xaml
    /// </summary>
    public partial class CategoriesManager : Window
    {
        public CategoriesManager()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = CategoryName.Text;
            string parentName = ParentCategory.Text;
            if(!((CategoriesViewModel)this.DataContext).AddCategory(categoryName, parentName))
            {
                MessageBox.Show("Can't add category.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem mi = (System.Windows.Controls.MenuItem)sender;
            Node node = (Node)mi.DataContext;

            if (!((CategoriesViewModel)this.DataContext).DeleteCategory(node))
            {
                MessageBox.Show("Can't delete category.");
            }
        }
    }
}
