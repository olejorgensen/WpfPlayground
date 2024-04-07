using Prism.Ioc;
using Prism.Modularity;

namespace DataGridWithCommunityToolKit.Modules;

public abstract class BaseModule : IModule
{
    public BaseModule() { }
    public abstract void OnInitialized(IContainerProvider containerProvider);
    public virtual void Initialize() { }
    public virtual void RegisterTypes(IContainerRegistry containerRegistry) { }
}
