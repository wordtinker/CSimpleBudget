using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;
using ViewModels;

namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for CategoriesReport.xaml
    /// </summary>
    public partial class CategoriesReport : Window
    {
        public CategoriesReport()
        {
            InitializeComponent();
        }

        private void Bar_Click(object sender, MouseButtonEventArgs e)
        {
            BarDataPoint bdp = (BarDataPoint)sender;
            BudgetBar bar = (BudgetBar)bdp.DataContext;
            ((CategoriesReportViewModel)DataContext).UpdateTransactions(bar);
        }
    }
}
