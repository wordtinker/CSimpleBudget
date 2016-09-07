using System.Windows;
using System.Windows.Controls;
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
            InitializeComponent();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // TODO
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
    }
}
