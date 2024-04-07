using System.Diagnostics;
using System.Windows;

namespace WpfPlayground;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        textBox.Text = "";
    }

    private void textBox_PreviewDragEnter(object sender, DragEventArgs e)
    {
        Trace.WriteLine(string.Format("{0}. sender: {1}, e: {2}", nameof(textBox_PreviewDragEnter), sender, e));

        if (!e.Data.GetDataPresent(DataFormats.StringFormat))
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            textBox.Tag = null;
            return;
        }

        Trace.WriteLine(string.Format("\tdata type: {0}", e.Data.GetType().Name));

        e.Effects = DragDropEffects.Copy;

        var dropText = TextDataFormatProvider.GetText(e.Data as DataObject);
        if (string.IsNullOrEmpty(dropText) || textBox.Text.Contains(dropText))
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            textBox.Tag = null;
            return;
        }

        textBox.Tag = new TextBoxPreviewDragState(textBox.Text, textBox.CaretIndex, dropText);

        if (!string.IsNullOrEmpty(textBox.Text) && !textBox.Text.EndsWith(System.Environment.NewLine))
            textBox.AppendText(System.Environment.NewLine);

        textBox.CaretIndex = textBox.Text.Length;
    }

    private void textBox_PreviewDragLeave(object sender, DragEventArgs e)
    {
        Trace.WriteLine(string.Format("{0}. sender: {1}, e: {2}", nameof(textBox_PreviewDragLeave), sender, e));

        if (textBox.Tag is TextBoxPreviewDragState state)
        {
            textBox.Text = state.OriginalText;
            textBox.CaretIndex = state.OriginalCaretIndex;
            textBox.Tag = null;
        }
    }

    private void textBox_PreviewDragOver(object sender, DragEventArgs e)
    {
        if (textBox.Tag is TextBoxPreviewDragState state)
        {
            Trace.WriteLine(string.Format("{0}. sender: {1}, e: {2}", nameof(textBox_PreviewDragOver), sender, e));

            if (!state.DidHandlePreviewDragOver)
            {
                state.DidHandlePreviewDragOver = true;
                textBox.Tag = state;
                if (!textBox.Text.Contains(state.DropText))
                {
                    textBox.CaretIndex = textBox.Text.Length;
                    textBox.AppendText(state.DropText);
                    textBox.CaretIndex = textBox.Text.Length;
                    textBox.ScrollToEnd();
                }
            }
            else
            {
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
    }

    private void textBox_PreviewDrop(object sender, DragEventArgs e)
    {
        if (textBox.Tag is TextBoxPreviewDragState state)
        {
            Trace.WriteLine(string.Format("{0}. sender: {1}, e: {2}", nameof(textBox_PreviewDrop), sender, e));

            if (textBox.Text.Contains(state.DropText))
            {
                e.Handled = true;
                textBox.CaretIndex = textBox.Text.Length;
                textBox.ScrollToEnd();
                textBox.Tag = null;
                return;
            }
            textBox.CaretIndex = textBox.Text.Length;
            textBox.ScrollToEnd();
            textBox.Tag = null;
        }
    }

    private void textBox_DragEnter(object sender, DragEventArgs e)
    {
        Trace.WriteLine(string.Format("{0}. sender: {1}, e: {2}", nameof(textBox_DragEnter), sender, e));

    }

    private void textBox_DragLeave(object sender, DragEventArgs e)
    {
        Trace.WriteLine(string.Format("{0}. sender: {1}, e: {2}", nameof(textBox_DragLeave), sender, e));

    }

    private void textBox_DragOver(object sender, DragEventArgs e)
    {
        Trace.WriteLine(string.Format("{0}. sender: {1}, e: {2}", nameof(textBox_DragOver), sender, e));

    }

    private void textBox_Drop(object sender, DragEventArgs e)
    {
        Trace.WriteLine(string.Format("{0}. sender: {1}, e: {2}", nameof(textBox_Drop), sender, e));

        if (textBox.Tag is TextBoxPreviewDragState state)
        {
            e.Handled = true;
            textBox.SelectedText = null;
            if (!textBox.Text.Contains(state.DropText))
            {
                textBox.Text = $"{state.OriginalText.TrimEnd()}{System.Environment.NewLine}{state.DropText}";
            }
            textBox.CaretIndex = textBox.Text.Length;
            textBox.ScrollToEnd();
            textBox.Tag = null;
        }
    }

    private void textBox_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
        Trace.WriteLine(string.Format("{0}. sender: {1}, e: {2}", nameof(textBox_PreviewGiveFeedback), sender, e));
    }
}
