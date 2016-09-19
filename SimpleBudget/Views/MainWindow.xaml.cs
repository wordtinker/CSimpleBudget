using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModels;


namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MainWindowService service = new MainWindowService(this);
            this.DataContext = new MainWindowViewModel(service);
            InitializeComponent();
        }

        public void Account_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow dgr = (DataGridRow)sender;
            AccountItem item = (AccountItem)dgr.DataContext;
            MainWindowViewModel vm = (MainWindowViewModel)this.DataContext;
            vm.ShowTransactionRoll(item);
        }
    }
}
