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

        public static (IHtmlDocumentContent? Contents, Exception? Error) GetHtmlDocumentContent(DataObject? dataObject)
        {
            (IHtmlDocumentContent? Contents, Exception? Error) result = (null, null);
            try
            {
                if (dataObject?.GetText(TextDataFormat.Html) is string htmlString && !string.IsNullOrWhiteSpace(htmlString))
                {
                    result.Contents = HtmlTextProvider.TryGetHtmlDocumentContent(htmlString);
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }
            return result;
        }

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

    /// <summary>
    /// Version: 0.9
    /// StartHTML: 0000000148
    /// EndHTML: 0000009764
    /// StartFragment: 0000000184
    /// EndFragment: 0000009728
    /// SourceURL: https://xyz.com/thebaseurl/
    /// </summary>
    public static partial class HtmlTextProvider
    {
        public static Uri? GetBaseUri(string htmlData)
        {
            var baseUri = new Uri(SourceUrl().Match(htmlData).Groups[1].Value.Trim());
            return baseUri;
        }

        public static HtmlDocument? TryGetHtmlDocumentContent(string htmlData)
        {
            HtmlDocument? result = null;
            try
            {
                var startHtmlIndex = int.Parse(StartHtml().Match(htmlData).Groups[1].Value);
                //var endHtmlIndex = int.Parse(Regex.Match(htmlData, @"EndHTML:(\d+)").Groups[1].Value);
                var baseUri = new Uri(SourceUrl().Match(htmlData).Groups[1].Value.Trim());
                var html = htmlData.Substring(startHtmlIndex);

                var result = new HtmlDocument();
                result.LoadHtml(html);
            }
            catch
            {
                // ignore
            }
            return result;
        }

        [GeneratedRegex(@"SourceURL:(.+)\n")]
        private static partial Regex SourceUrl();

        [GeneratedRegex(@"StartHTML:(\d+)")]
        private static partial Regex StartHtml();
    }
}
