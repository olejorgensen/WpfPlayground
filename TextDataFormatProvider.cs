using System.Collections.Generic;
using System.Windows;

namespace WpfPlayground
{
    public static class TextDataFormatProvider
    {
        public readonly static List<TextDataFormat> PreferedFormats = new()
        {
            TextDataFormat.Html,
            TextDataFormat.Text
        };

        public static string? GetText(DataObject? obj)
        {
            if (obj is not null)
                foreach (var preferedFormat in PreferedFormats)
                {
                    if (obj.GetText(preferedFormat) is string dropText && !string.IsNullOrWhiteSpace(dropText))
                    {
                        return dropText;
                    }
                }
            return null;
        }
    }
}
