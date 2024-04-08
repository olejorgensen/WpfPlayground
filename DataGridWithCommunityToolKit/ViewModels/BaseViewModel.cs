namespace DataGridWithCommunityToolKit.ViewModels;

#region Usings
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
        this.statusMessage = string.Empty;
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

    public static readonly Task<int> DoNothing = Task.FromResult<int>(0);

    [ObservableProperty]
    protected bool canReload = true;

    [RelayCommand(CanExecute = nameof(CanReload))]
    protected abstract Task<int> Reload();

    #endregion Reload

    #endregion

    #region CTK Events

    protected virtual async Task OnIsVisibleChangedInternal(bool value)
    {
        if (firstLoad || value)
        {
            firstLoad = false;
            StatusMessage = "Please wait...";
            await Reload();
            StatusMessage = string.Empty;
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