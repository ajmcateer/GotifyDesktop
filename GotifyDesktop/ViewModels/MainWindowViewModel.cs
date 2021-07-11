using GotifyDesktop.Infrastructure;
using Serilog;
using ReactiveUI;
using System.Threading.Tasks;
using System.Reactive.Linq;
using GotifyDesktop.Interfaces;
using System.Reactive.Disposables;
using System;
using gotifySharp;
using Websocket.Client;
using GotifyDesktop.Models;

namespace GotifyDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IActivatableViewModel, IScreen
    {
        public ViewModelActivator Activator { get; }

        ServerViewModelFactory _serverViewModelFactory;

        ISettingsService _settingsService;

        SettingsViewModel _settingViewModel;

        public RoutingState Router { get; } 

        public MainWindowViewModel(ServerViewModelFactory serverViewModelFactory,
            SettingsViewModel settingViewModel,
            ISettingsService settingsService,
            RoutingState routingState)
        {
            _serverViewModelFactory = serverViewModelFactory;
            _settingsService = settingsService;
            _settingViewModel = settingViewModel;
            Router = routingState;
            Activator = new ViewModelActivator();

            this.WhenActivated(async (CompositeDisposable disposables) =>
            {
                await OnActivationAsync();
                Disposable
                    .Create(() => { /* handle deactivation */ })
                    .DisposeWith(disposables);
            });
        }

        public MainWindowViewModel()
        {
        }

        public async Task OnActivationAsync()
        {
            ServerInfo server = _settingsService.GetSettings();
            if (String.IsNullOrEmpty(server.Url))
            {
                await Router.Navigate.Execute(_settingViewModel);
            }
        }
    }
}