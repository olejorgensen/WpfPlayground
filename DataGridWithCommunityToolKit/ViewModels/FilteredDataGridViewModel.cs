namespace DataGridWithCommunityToolKit.ViewModels;

#region Usings

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataGridWithCommunityToolKit.Views;


# endregion

public partial class FilteredDataGridViewModel<T> : BaseViewModel
    where T : class
{
    #region Fields
    private readonly ObservableCollection<T> _items;
    private readonly ICollectionView _list;
    private readonly int _max = 10000; 
    #endregion

    #region CTOR
    public FilteredDataGridViewModel(IMessenger messenger) : base(messenger)
    {
        _items = new ObservableCollection<T>([]);
        _list = new InternalCollectionView<T>(this, _items)
        {
            Filter = CreateStringFilter()
        };
    }
    #endregion

    #region Properties

    public IFilteredDataGridView<T> View { get; set; }

    public T? SelectedItem
    {
        get
        {
            return (T)List.CurrentItem;
        }
        set
        {
            List.MoveCurrentTo(value);
        }
    }

    #region CTK Properties

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToolTipText))]
    private int itemCount = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToolTipText))]
    private int filteredCount = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFilterRegex))]
    [NotifyPropertyChangedFor(nameof(FilterRegex))]
    private string filterText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilterText))]
    [NotifyPropertyChangedFor(nameof(FilterRegex))]
    private bool isFilterRegex = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFilterRegex))]
    [NotifyPropertyChangedFor(nameof(FilterText))]
    private Regex? filterRegex = null;

    partial void OnFilterTextChanged(string? oldValue, string newValue)
    {
        if (string.Compare(oldValue ?? string.Empty, newValue, StringComparison.InvariantCultureIgnoreCase) == 0)
            return;

        _list.Filter = CreateStringFilter();
    }

    public string ToolTipText
    {
        get
        {
            if (itemCount == 0)
                return "List is empty...";
            return filteredCount == itemCount
                ? $"{itemCount}"
                : $"{filteredCount}/{itemCount}";
        }
    }

    #endregion

    #endregion

    #region CTK Commands

    [RelayCommand]
    private async Task Populate()
    {
        try
        {
            await Reload();
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    [RelayCommand]
    private void RemoveFilteredItems()
    {
        if (string.IsNullOrEmpty(filterText)) return;

        var itemsToRemove = ((InternalCollectionView<T>)_list).GetFilteredItems();
        if (itemsToRemove.Any())
        {
            foreach (var item in itemsToRemove)
                _items.Remove(item);

            FilterText = string.Empty;
        }
    }

    [RelayCommand]
    private void RemoveSelectedItems()
    {
        try
        {
            var index = View.SelectedIndex;
            var itemsToRemove = View.GetSelectedItems();
            foreach (var item in itemsToRemove)
                _items.Remove(item);
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    [RelayCommand]
    private void SelectAll()
    {
        View.SelectAll();
    }

    #endregion

    #region Data

    protected override async Task<int> Reload()
    {
        if (Dispatcher is not null && Dispatcher.CheckAccess() == false)
        {
            var dispatcherTask = await Dispatcher.InvokeAsync(Reload, DispatcherPriority.Background);
            return await dispatcherTask;
        }

        CanReload = false;
        StatusMessage = "Please wait...";
        var result = -1;

        try
        {
            var newItems = CreateItemsHelper(_max);
            _items.AddRange
            (
                newItems
            );

            StatusMessage = string.Empty;

            result = _max - _items.Count;
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
        finally
        {
            CanReload = true;
        }

        return result;
    }

    /// <summary>
    /// Update this through Items
    /// </summary>
    public ICollectionView List
    {
        get
        {
            return _list;
        }
    }

    public class InternalCollectionView<Y>(FilteredDataGridViewModel<Y> owner, IList list) : ListCollectionView(list), ICollectionView
        where Y : class
    {
        public FilteredDataGridViewModel<Y> Owner { get; } = owner;

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
            if (args.PropertyName?.EndsWith("Count") == true)
            {
                Owner.FilteredCount = this.Count;
                Owner.ItemCount = this.SourceCollection.Cast<Y>().Count();
            }
        }

        public List<Y> GetFilteredItems()
        {
            return base.InternalList.Cast<Y>().ToList();
        }
    }

    private IEnumerable<T> CreateItemsHelper(int max)
    {
        var newItems = new List<Uri>(max);
        for (var i = 1; i <= max; i++)
        {
            newItems.Add(new Uri($"https://item{i}.com/item{i}/?q={i}"));
        }
        return newItems.Cast<T>().Where(l => !_items.Contains(l));
    }

    private Predicate<object> CreateStringFilter()
    {
        return (o) =>
        {
            if (o is Uri uri)
                return uri.AbsoluteUri.Contains(filterText, StringComparison.InvariantCultureIgnoreCase) == true;
            else
                return false;
        };
    }

    #endregion

}
