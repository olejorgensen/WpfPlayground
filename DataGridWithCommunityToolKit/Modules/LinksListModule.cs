using DataGridWithCommunityToolKit.Views;
using Prism.Ioc;
using Prism.Regions;

namespace DataGridWithCommunityToolKit.Modules;
public class LinksListViewModule : BaseModule
{
    public override void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
        regionManager.RegisterViewWithRegion("LinksListViewRegion", typeof(LinksListView));
    }
}

