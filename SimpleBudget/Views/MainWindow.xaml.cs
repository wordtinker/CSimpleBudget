using System.Windows;
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
    }
}
