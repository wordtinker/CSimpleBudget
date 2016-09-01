using System.Windows;
using System.Windows.Input;

namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for TransactionRoll.xaml
    /// </summary>
    public partial class TransactionRoll : Window
    {
        public TransactionRoll()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            MessageBox.Show("Add");
        }

        private void Transaction_DoubleClick(object Sender, MouseButtonEventArgs e)
        {
            // TODO
            MessageBox.Show("Edit");
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            MessageBox.Show("Delete");
        }
    }
}
