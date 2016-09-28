using System.Windows;
using System.Windows.Controls;
using ViewModels;

namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for AccTypeManager.xaml
    /// </summary>
    public partial class AccTypeManager : Window
    {
        public AccTypeManager()
        {
            InitializeComponent();
        }

        private void DeleteAccountType_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            AccTypeItem item = (AccTypeItem)mi.DataContext;
            if (!((AccTypeViewModel)this.DataContext).DeleteAccType(item))
            {
                MessageBox.Show("Can't delete account type.");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string accTypeName = AccTypeName.Text;
            if (!((AccTypeViewModel)this.DataContext).AddAccType(accTypeName))
            {
                MessageBox.Show("Can't add account type.");
            }
        }
    }
}
