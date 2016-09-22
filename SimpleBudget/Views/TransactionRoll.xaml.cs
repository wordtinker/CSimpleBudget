using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModels;

namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for TransactionRoll.xaml
    /// </summary>
    public partial class TransactionRoll : Window
    {
        public TransactionRoll()
        {
            Closing += (sender, e) =>
            {
                ((TransactionRollViewModel)DataContext).Close();
            };

            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ((TransactionRollViewModel)DataContext).ShowTransactionEditor();
        }

        private void Transaction_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow dgr = (DataGridRow)sender;
            TransactionItem item = (TransactionItem)dgr.DataContext;
            ((TransactionRollViewModel)DataContext).ShowTransactionEditor(item);
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            TransactionItem item = (TransactionItem)mi.DataContext;
            if (!((TransactionRollViewModel)this.DataContext).DeleteTransaction(item))
            {
                MessageBox.Show("Can't delete transaction.");
            }
        }
    }
}
