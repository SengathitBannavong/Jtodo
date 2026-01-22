using System;
using System.Globalization;
using System.Windows.Data;

namespace Jtodo.Commands
{
    public class SortIndicatorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3)
                return "";

            var currentColumn = values[0] as string;
            var columnName = values[1] as string;
            var isAscending = values[2] as bool?;

            if (string.IsNullOrEmpty(currentColumn) || string.IsNullOrEmpty(columnName))
                return "";

            // If this is the active sort column
            if (currentColumn.Equals(columnName, StringComparison.OrdinalIgnoreCase))
            {
                return isAscending == true ? "↑" : "↓";
            }

            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
