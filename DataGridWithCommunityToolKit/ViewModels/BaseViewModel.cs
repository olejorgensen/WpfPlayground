namespace DataGridWithCommunityToolKit.ViewModels;

#region Usings
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

public abstract partial class BaseViewModel : ObservableObject
{
    #region Fields
    internal Dispatcher? Dispatcher;
    #endregion

    #region CTOR

    public BaseViewModel(IMessenger messenger)
    {
        ArgumentNullException.ThrowIfNull(messenger);

        this.Messenger = messenger;
    }

    #endregion CTOR

    #region Properties

    [ObservableProperty]
    private bool hasUserCancelled;

    [ObservableProperty]
    private IMessenger messenger;

    [ObservableProperty]
    protected bool isLoaded;

    [ObservableProperty]
    protected bool isVisible;

    public string ErrorMessage => LastException?.Message ?? string.Empty;
    public string ErrorInfo => LastException?.ToString() ?? string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErrorMessage))]
    [NotifyPropertyChangedFor(nameof(ErrorInfo))]
    protected Exception? lastException;

    [ObservableProperty]
    private string statusMessage;

    #endregion

    #region Commands

    #region Reload

    protected bool firstLoad = true;

    protected bool isListDirty = true;
    public static readonly Task<int> DoNothing = Task.FromResult<int>(0);

    /// <summary>
    /// Default impl. that does nothing
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    public abstract Task<int> Reload(string? filter = null);

    #endregion Reload

    #endregion

    #region CommunityToolkit Events

    protected virtual async Task OnIsVisibleChangedInternal(bool value)
    {
        if (firstLoad || value)
        {
            await Reload(null).ConfigureAwait(false);
        }
    }

    partial void OnIsVisibleChanged(bool value)
    {
        Task.Run(() => OnIsVisibleChangedInternal(value));
    }

    #endregion Events

    #region Helpers

    #region Notify Property Changed

    protected void NotifyPropertyChanging(string propertyName)
    {
        if (Dispatcher is not null && Dispatcher.CheckAccess() == false)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(NotifyPropertyChanging), propertyName);
            return;
        }
        OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
    }

    protected void NotifyPropertyChanged(string propertyName)
    {
        if (Dispatcher is not null && Dispatcher.CheckAccess() == false)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(NotifyPropertyChanged), propertyName);
            return;
        }
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    #endregion Notify Property Changed

    #endregion Helpers
}