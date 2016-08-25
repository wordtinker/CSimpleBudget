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
            // TODO !!!
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = (ListBox)sender;
            Item si = (Item)box.SelectedItem;
            ((AccountsViewModel)this.DataContext).UpdateSelection(si);
        }
    }
}
