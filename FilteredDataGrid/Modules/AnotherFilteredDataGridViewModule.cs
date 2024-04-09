using FilteredDataGrid.Views;
using Prism.Ioc;
using Prism.Regions;

namespace FilteredDataGrid.Modules;

public class AnotherFilteredDataGridViewModule : BaseModule
{
    public override void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
        regionManager.RegisterViewWithRegion("AnotherFilteredDataGridViewRegion", typeof(FilteredDataGridView));
    }
}

