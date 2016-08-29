using System;
using System.Windows;
using System.Windows.Controls;
using ViewModels;

namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for AccountsManager.xaml
    /// </summary>
    public partial class AccountsManager : Window
    {
        public AccountsManager()
        {
            InitializeComponent();
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            Item item = (Item)mi.DataContext;
            if (!((AccountsViewModel)this.DataContext).DeleteAccount(item))
            {
                MessageBox.Show("Can't delete account.");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string accName = AccName.Text;
            if (!((AccountsViewModel)this.DataContext).AddAccount(accName))
            {
                MessageBox.Show("Can't add account.");
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = (ListBox)sender;
            Item si = (Item)box.SelectedItem;
            ((AccountsViewModel)this.DataContext).UpdateSelection(si);
        }
    }
}
