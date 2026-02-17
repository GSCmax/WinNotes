using System.Globalization;
using System.Windows.Data;
using Application = System.Windows.Application;

namespace WinNotes.Helpers
{
    public class Bool2ResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Application.Current.FindResource(((string)parameter).Split(',')[0]);
            else
                return Application.Current.FindResource(((string)parameter).Split(',')[1]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
