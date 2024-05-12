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

    public static class HtmlTextProvider
    {
        public static Uri? GetBaseUri(string htmlData)
        {
            var baseUri = new Uri(Regex.Match(htmlData, @"SourceURL:(.+)\n").Groups[1].Value.Trim());
            return baseUri;
        }

        public static IHtmlDocumentContent? TryGetHtmlDocumentContent(string htmlData)
        {
            var startHtmlIndex = int.Parse(Regex.Match(htmlData, @"StartHTML:(\d+)").Groups[1].Value);
            //var endHtmlIndex = int.Parse(Regex.Match(htmlData, @"EndHTML:(\d+)").Groups[1].Value);
            var baseUri = new Uri(Regex.Match(htmlData, @"SourceURL:(.+)\n").Groups[1].Value.Trim());
            var html = htmlData.Substring(startHtmlIndex);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var service = new HtmlDocumentContentService
            {
                Document = doc,
                BaseUri = baseUri
            };

            return service.GetContents();
        }
    }
}
