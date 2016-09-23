using System.Windows;

namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for BudgetManagerCopyRequest.xaml
    /// </summary>
    public partial class BudgetManagerCopyRequest : Window
    {
        public BudgetManagerCopyRequest()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
