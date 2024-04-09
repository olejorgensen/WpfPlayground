namespace FilteredDataGrid.Views;

using System.Collections.Generic;

public interface IFilteredListView<T>
    where T : class
{
    int SelectedIndex { get; set; }
    IList<T> GetSelectedItems();
    int SelectedItemsCount { get; }
    void SelectAll();
}
