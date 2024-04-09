using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlayground
{
    public class TextBoxPreviewDragState
    {
        public TextBoxPreviewDragState(string originalText, int originalCaretIndex, string dropText)
        {
            OriginalText = originalText;
            OriginalCaretIndex = originalCaretIndex;
            DropText = dropText;
        }

        public string OriginalText { get; }

        public int OriginalCaretIndex { get; }

        public string DropText { get; }

        public bool DidHandlePreviewDragOver { get; set; }
    }
}
