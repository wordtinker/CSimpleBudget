using System.Windows;

namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for Transaction_Editor.xaml
    /// </summary>
    public partial class TransactionEditor : Window
    {
        public TransactionEditor()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
