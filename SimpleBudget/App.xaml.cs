using System.Globalization;
using System.Windows;

namespace SimpleBudget
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Creating a Global culture specific to our application.
            CultureInfo cultureInfo = new CultureInfo("en-US");

            // Creating the DateTime Information specific to our application.
            DateTimeFormatInfo dateTimeInfo = new DateTimeFormatInfo();

            // Defining various date and time formats.
            dateTimeInfo.DateSeparator = "/";
            dateTimeInfo.LongDatePattern = "dd/MM/yyyy";
            dateTimeInfo.ShortDatePattern = "dd/MM/yyyy";

            // Setting application wide date time format.
            cultureInfo.DateTimeFormat = dateTimeInfo;

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}
