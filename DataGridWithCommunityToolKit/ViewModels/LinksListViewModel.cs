namespace DataGridWithCommunityToolKit.ViewModels;

#region Usings

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataGridWithCommunityToolKit.Views;


# endregion

public partial class LinksListViewModel<T> : BaseViewModel
    where T : class
{
    #region Fields
    private readonly ObservableCollection<T> _items;
    private readonly ICollectionView _list;
    private readonly int _max = 10000; 
    #endregion

    #region CTOR
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public LinksListViewModel(IMessenger messenger) : base(messenger)
    {
        _items = new ObservableCollection<T>([]);
        _list = new InternalCollectionView<T>(this, _items)
        {
            Filter = CreateStringFilter()
        };
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

    #region Properties

    public LinksListView View { get; set; }

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
    [NotifyPropertyChangedFor(nameof(ToolTip))]
    private int itemCount = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToolTip))]
    private int filteredCount = 0;

    [ObservableProperty]
    private string filterText = string.Empty;

    partial void OnFilterTextChanged(string? oldValue, string newValue)
    {
        if (string.Compare(oldValue ?? string.Empty, newValue, StringComparison.InvariantCultureIgnoreCase) == 0)
            return;

        _list.Filter = CreateStringFilter();
    }

#pragma warning disable MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
    public string ToolTip
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
#pragma warning restore MVVMTK0034 // Direct field reference to [ObservableProperty] backing field

    #endregion

    #endregion

    #region CTK Commands

    [RelayCommand]
    private void Populate()
    {
        try
        {
            var newItems = GetItems(_max);
            _items.AddRange
            (
                newItems
            );
        }
        catch (Exception ex)
        {
            LastException = ex;
        }
    }

    [RelayCommand]
    private void RemoveFilteredItems()
    {
#pragma warning disable MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
        if (string.IsNullOrEmpty(filterText)) return;
#pragma warning restore MVVMTK0034 // Direct field reference to [ObservableProperty] backing field

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
            var itemsToRemove = View.GetSelectedItems<T>();
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

    public override Task<int> Reload(string? filter = null)
    {
        return DoNothing;
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

    public class InternalCollectionView<Y>(LinksListViewModel<Y> owner, IList list) : ListCollectionView(list), ICollectionView
        where Y : class
    {
        public LinksListViewModel<Y> Owner { get; } = owner;

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
            if (args.PropertyName?.EndsWith("Count") == true)
            {
                Owner.FilteredCount = this.Count;
                Owner.ItemCount = this.SourceCollection.Cast<T>().Count();
            }
        }

        public List<Y> GetFilteredItems()
        {
            return base.InternalList.Cast<Y>().ToList();
        }
    }

    private IEnumerable<T> GetItems(int max)
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

    #region Overrides

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(IsLoaded) && isLoaded)
        {
            PopulateCommand.Execute(null);
        }
    }

    #endregion
}
