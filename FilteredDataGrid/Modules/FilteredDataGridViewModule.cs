namespace FilteredDataGrid.Modules;

using FilteredDataGrid.Views;
using Prism.Ioc;
using Prism.Regions;

public class FilteredDataGridViewModule : BaseModule
{
    public override void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
        regionManager.RegisterViewWithRegion("FilteredDataGridViewRegion", typeof(FilteredDataGridView));
    }
}

