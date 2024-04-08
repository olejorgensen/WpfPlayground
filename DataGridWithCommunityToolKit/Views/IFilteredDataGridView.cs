namespace DataGridWithCommunityToolKit.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IFilteredDataGridView<T>
    where T : class
{
    int SelectedIndex { get; set; }
    IList<T> GetSelectedItems();
    void SelectAll();
    IFilteredDataGridView<T> View { get; }
}
