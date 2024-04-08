using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using DataGridWithCommunityToolKit.ViewModels;

namespace DataGridWithCommunityToolKit.Views
{
    /// <summary>
    /// Interaction logic for FilteredDataGridView.xaml
    /// </summary>
    public partial class FilteredDataGridView: UserControl, IFilteredDataGridView<Uri>
    {
        public FilteredDataGridView(IMessenger messenger)
        {
            InitializeComponent();
            IsVisibleChanged += (o, e) =>
            {
                if (e.NewValue is bool isVisible)
                {
                    VM.IsVisible = isVisible;
                }
            };
            Loaded += (o, e) =>
            {
                filterTextBox.Focus();
                VM.IsLoaded = true;
            };
            Unloaded += (o, e) =>
            {
                VM.IsLoaded = false;
            };
            filterTextBox.KeyUp += (o, e) =>
            {
               if (e.Key == System.Windows.Input.Key.Delete && e.KeyStates == System.Windows.Input.KeyStates.Toggled)
                    VM.FilterText = string.Empty;
            };
            dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;

            var vm = new FilteredDataGridViewModel<Uri>(messenger);
            DataContext = vm;
            vm.Dispatcher = this.Dispatcher;
            vm.View = this;
        }

        public IList<Uri> GetSelectedItems()
        {
            var list = dataGrid.SelectedItems.Cast<Uri>().ToList();
            return list;
        }

        public void SelectAll()
        {
            dataGrid.SelectAll();
        }

        public int SelectedIndex
        {
            get => dataGrid.SelectedIndex;
            set
            {
                try
                {
                    dataGrid.SelectedIndex = Math.Min(value, dataGrid.Items.Count - 1);
                    dataGrid.Focus();
                }
                catch (Exception ex)
                {
                    VM.LastException = ex;
                }
            }
        }

        private FilteredDataGridViewModel<Uri> VM => (FilteredDataGridViewModel<Uri>)DataContext;

        public IFilteredDataGridView<Uri> View
        {
            get => this;
        }
    }
}
