﻿using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;

namespace NavigationAndEditing;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void InitializeShell(Window shell)
    {
        base.InitializeShell(shell);
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance<IMessenger>(WeakReferenceMessenger.Default);
    }

    protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
    {
        base.ConfigureDefaultRegionBehaviors(regionBehaviors);
    }

    //protected override IModuleCatalog CreateModuleCatalog()
    //{
    //    var compiledPath = @"C:\dev\WpfPlayground\FilteredDataGrid\Modules\";
    //    return new DirectoryModuleCatalog() { ModulePath = compiledPath };
    //}

    //protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    //{
    //    moduleCatalog.AddModule<FilteredDataGridViewModule>();
    //    moduleCatalog.AddModule<AnotherFilteredDataGridViewModule>();

    //}
}

