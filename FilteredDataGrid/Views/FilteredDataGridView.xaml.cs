namespace FilteredDataGrid.Views;

using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using FilteredDataGrid.ViewModels;
using System.Windows;

/// <summary>
/// Interaction logic for FilteredDataGridView.xaml
/// </summary>
public partial class FilteredDataGridView: UserControl, IFilteredListView<Uri>
{
    #region Constructor

    public FilteredDataGridView(IMessenger messenger)
    {
        InitializeComponent();

        var vm = new FilteredDataGridViewModel<Uri>(messenger);
        vm.Dispatcher = this.Dispatcher;
        vm.View = this;

        DataContext = vm;

        IsVisibleChanged += (o, e) =>
        {
            if (e.NewValue is bool isVisible)
            {
                VM.IsVisible = isVisible;
            }
        };
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
    } 

    #endregion

    #region IFilteredListView<Uri>

    public IList<Uri> GetSelectedItems()
    {
        var list = dataGrid.SelectedItems.Cast<Uri>().ToList();
        return list;
    }

    public int SelectedItemsCount
    {
        get
        {
            return dataGrid.SelectedItems.Count;
        }
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
                dataGrid.SelectedIndex = Math.Clamp(value, -1, dataGrid.Items.Count - 1);
                dataGrid.Focus();
            }
            catch (Exception ex)
            {
                VM.LastException = ex;
            }
        }
    }

    #endregion

    #region Helpers

    private FilteredDataGridViewModel<Uri> VM => (FilteredDataGridViewModel<Uri>)DataContext;

    #endregion

    #region UI Events

    private void Register()
    {
        Unregister();
        dataGrid.SelectionChanged += OnDataGridSelectionChanged;
        filterTextBox.KeyUp += OnFilterTextKeyUp;
    }

    private void Unregister()
    {
        dataGrid.SelectionChanged -= OnDataGridSelectionChanged;
        filterTextBox.KeyUp -= OnFilterTextKeyUp;
    }

    private void OnLoaded(object o, RoutedEventArgs e)
    {
        try
        {
            Register();
            filterTextBox.Focus();
            VM.IsLoaded = true;
        }
        catch (Exception ex)
        {
            VM.LastException = ex;
        }
    }

    private void OnUnloaded(object o, RoutedEventArgs e)
    {
        try
        {
            Unregister();
            VM.IsLoaded = false;
        }
        catch (Exception ex)
        {
            VM.LastException = ex;
        }
    }

    private void OnDataGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            VM.SelectedItemsCount = this.SelectedItemsCount;
        }
        catch (Exception ex)
        {
            VM.LastException = ex;
        }
    }

    private void OnFilterTextKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        try
        {
            if (e.Key == System.Windows.Input.Key.Delete && e.KeyStates == System.Windows.Input.KeyStates.Toggled)
            {
                VM.FilterText = string.Empty;
            }
        }
        catch (Exception ex)
        {
            VM.LastException = ex;
        }
    } 

    #endregion
}
