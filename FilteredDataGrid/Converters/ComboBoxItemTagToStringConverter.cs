namespace FilteredDataGrid.Converters;

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

public class ComboBoxItemTagToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ComboBoxItem comboBoxItem)
        {
            return comboBoxItem.Tag;
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ComboBoxItem comboBoxItem)
        {
            return comboBoxItem.Tag;
        }
        return string.Empty;
    }
}
