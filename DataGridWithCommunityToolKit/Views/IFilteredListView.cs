namespace DataGridWithCommunityToolKit.Views;

using System.Collections.Generic;

public interface IFilteredListView<T>
    where T : class
{
    int SelectedIndex { get; set; }
    IList<T> GetSelectedItems();
    void SelectAll();
    IFilteredListView<T> View { get; }
}
