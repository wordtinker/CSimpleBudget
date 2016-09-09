using System.Windows;

namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for BudgetRecordEditor.xaml
    /// </summary>
    public partial class BudgetRecordEditor : Window
    {
        public BudgetRecordEditor()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
