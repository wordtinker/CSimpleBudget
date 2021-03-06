﻿using System.Windows;
using System.Windows.Controls;
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
            CategoryNode parent = (CategoryNode)ParentCategory.SelectedItem;
            if(!((CategoriesViewModel)this.DataContext).AddCategory(categoryName, parent))
            {
                MessageBox.Show("Can't add category.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            CategoryNode node = (CategoryNode)mi.DataContext;

            if (!((CategoriesViewModel)this.DataContext).DeleteCategory(node))
            {
                MessageBox.Show("Can't delete category.");
            }
        }
    }
}
