namespace FilteredDataGrid.ViewModels;

#region Usings
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FilteredDataGrid.Views;
# endregion

public partial class FilteredDataGridViewModel<T> : BaseViewModel
    where T : class
{
    #region Fields
    private readonly ObservableCollection<T> _items;
    private readonly ICollectionView _list;
    #endregion

    #region CTOR
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public FilteredDataGridViewModel(IMessenger messenger) : base(messenger)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _items = new ObservableCollection<T>([]);
        _list = new InternalCollectionView<T>(this, _items)
        {
            Filter = CreateStringFilter()
        };
    }
    #endregion

    #region Properties

    public IFilteredListView<T> View { get; set; }

    public T? SelectedItem
    {
        get => (T)_list.CurrentItem;
        set
        {
            try
            {
                _list.MoveCurrentTo(value);
            }
            catch (Exception ex)
            {
                LastException = ex;
            }
        }
    }

    #region CTK Properties

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToolTipText))]
    private int itemCount = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToolTipText))]
    [NotifyCanExecuteChangedFor(nameof(RemoveFilteredItemsCommand))]
    private int filteredCount = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFilterRegex))]
    [NotifyCanExecuteChangedFor(nameof(RemoveFilteredItemsCommand))]
    private string filterText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilterText))]
    [NotifyCanExecuteChangedFor(nameof(RemoveFilteredItemsCommand))]
    private bool isFilterRegex = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilterText))]
    [NotifyCanExecuteChangedFor(nameof(RemoveFilteredItemsCommand))]
    private Regex filterRegex = new(".");

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveSelectedItemsCommand))]
    private int selectedItemsCount = 0;

    private string _selectedExpression = string.Empty;
    public string SelectedExpression
    {
        get => _selectedExpression;
        set
        {
            try
            {
                if (SetProperty(ref _selectedExpression, value, nameof(SelectedExpression)))
                {
                    var query = new Regex(value, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    IsFilterRegex = true;
                    FilterText = value;
                    FilterRegex = query;
                    _selectedExpression = value;
                }
            }
            catch (Exception ex)
            {
                LastException = ex;
                IsFilterRegex = false;
                FilterText = value;
                FilterRegex = new(".");
                _selectedExpression = value;
            }
        }
    }


    [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0034:Direct field reference to [ObservableProperty] backing field", Justification = "<Pending>")]
    partial void OnFilterTextChanged(string value)
    {
        if (isFilterRegex && TryCreateFilterRegex)
        {
            _list.Filter = CreateRegexFilter();
        }
        else
        {
            _list.Filter = CreateStringFilter();
        }
        OnPropertyChanged(nameof(List));
    }

    partial void OnIsFilterRegexChanged(bool value)
    {
        OnFilterTextChanged(filterText);
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

    public bool CanRemoveFilteredItems
    {
        get
        {
            return canReload && !string.IsNullOrEmpty(filterText);
        }
    }

    [RelayCommand(CanExecute = nameof(CanRemoveFilteredItems))]
    private void RemoveFilteredItems()
    {
        CanReload = false;

        try
        {
            if (string.IsNullOrEmpty(filterText)) return;

            StatusMessage = "Removing filtered items...";

            var itemsToRemove = ((InternalCollectionView<T>)_list).GetFilteredItems();
            var oldIndex = View.SelectedIndex;
            if (itemsToRemove.Any())
            {
                foreach (var item in itemsToRemove)
                    _items.Remove(item);

                CanReload = true;
                View.SelectedIndex = -1;
                View.SelectedIndex = oldIndex;
            }

            StatusMessage = string.Empty;
        }
        catch (Exception ex)
        {
            LastException = new ApplicationException($"Filtered items could not be removed: {ex.Message}", ex);
        }
        finally
        {
            CanReload = true;
            RemoveSelectedItemsCommand.NotifyCanExecuteChanged();
        }
    }

    public bool CanRemoveSelectedItems
    {
        get
        {
            return canReload && View.SelectedItemsCount > 0;
        }
    }

    [RelayCommand(CanExecute = nameof(CanRemoveSelectedItems))]
    private void RemoveSelectedItems()
    {
        CanReload = false;

        try
        {
            StatusMessage = "Removing selection...";

            var index = View.SelectedIndex;
            var itemsToRemove = View.GetSelectedItems();
            foreach (var item in itemsToRemove)
                _items.Remove(item);

            CanReload = true;

            StatusMessage = string.Empty;

            View.SelectedIndex = -1;
            View.SelectedIndex = index;
        }
        catch (Exception ex)
        {
            LastException = new ApplicationException($"Selected items could not be removed: {ex.Message}", ex);
        }
        finally
        {
            CanReload = true;
        }
    }

    [RelayCommand]
    private void SelectAll()
    {
        View.SelectAll();
    }

    [RelayCommand]
    private void ToggleIsFilterRegex()
    {
        IsFilterRegex = !IsFilterRegex;
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
            var oldCount = _items.Count;
            var max = (new Random(DateTime.Now.Second)).Next(1, 200);
            var newItems = CreateItemsHelper(max);
            _items.AddRange
            (
                newItems
            );

            result = Math.Abs(oldCount - _items.Count);

            StatusMessage = result == 0 ? "No changes..." : $"{result} new items found...";

        }
        catch (Exception ex)
        {
            LastException = new ApplicationException($"Error when fetching data: {ex.Message}", ex);
        }
        finally
        {
            CanReload = true;

            RemoveFilteredItemsCommand.NotifyCanExecuteChanged();
            RemoveSelectedItemsCommand.NotifyCanExecuteChanged();
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
                Owner.FilteredCount = Math.Max(0, this.Count - 1);
                Owner.ItemCount = this.SourceCollection.Cast<Y>().Count();
            }
        }

        public List<Y> GetFilteredItems()
        {
            return base.InternalList.Cast<Y>().ToList();
        }
    }

    private IEnumerable<T> CreateItemsHelper(int maxDays)
    {
        var newItems = new List<Uri>(maxDays);
        for (var i = 1; i <= maxDays; i++)
        {
            newItems.Add(new Uri($"https://inventory.com/items?days={i}"));
        }
        return newItems.Cast<T>().Where(l => !_items.Contains(l));
    }

    private Predicate<object> CreateRegexFilter()
    {
        return (o) =>
        {
            if (o is Uri uri)
                return filterRegex.IsMatch(uri.AbsoluteUri) == true;
            else
                return false;
        };
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

    #region Helpers

    private bool TryCreateFilterRegex
    {
        get
        {
            var result = false;
            try
            {
                var query = new Regex(filterText, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                filterRegex = query;
                StatusMessage = string.Empty;
                result = true;
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                LastException = ex;
            }
            return result;
        }
    }

    #endregion
}
