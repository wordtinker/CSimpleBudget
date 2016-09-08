using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModels;

namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for BudgetManager.xaml
    /// </summary>
    public partial class BudgetManager : Window
    {
        public BudgetManager()
        {
            Closing += (sender, e) =>
            {
                ((BudgetManagerViewModel)DataContext).Close();
            };

            InitializeComponent();
        }

        private void Record_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem li = (ListViewItem)sender;
            RecordItem item = (RecordItem)li.DataContext;
            ((BudgetManagerViewModel)DataContext).ShowTransactionEditor(item);
        }

        private void Month_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = (sender as ComboBox).SelectedIndex;
            ((BudgetManagerViewModel)this.DataContext).CurrenMonthChanged(index);
        }

        private void Year_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sYear = (sender as ComboBox).SelectedItem.ToString();
            if (sYear != null)
            {
                int year = int.Parse(sYear);
                ((BudgetManagerViewModel)this.DataContext).CurrentYearChanged(year);
            }
        }

        private void DeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            RecordItem item = (RecordItem)mi.DataContext;
            if (!((BudgetManagerViewModel)this.DataContext).DeleteRecord(item))
            {
                MessageBox.Show("Can't delete budget record.");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ((BudgetManagerViewModel)DataContext).ShowTransactionEditor();
        }
    }
}
