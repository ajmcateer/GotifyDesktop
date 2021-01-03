using GotifyDesktop.Infrastructure;
using Serilog;
using ReactiveUI;
using System.Threading.Tasks;
using System.Reactive.Linq;
using GotifyDesktop.Interfaces;
using System.Reactive.Disposables;
using System;

namespace GotifyDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, ICustomScreen, IActivatableViewModel
    {
        ILogger logger;
        public ViewModelActivator Activator { get; }

        ServerViewModelFactory _serverViewModelFactory;
        SettingsViewModel _settingsViewModel;
        public RoutingState Router { get; } = new RoutingState();

        public MainWindowViewModel(ServerViewModelFactory serverViewModelFactory, 
            SettingsViewModel settingsViewModel, 
            ViewModelActivator viewModelActivator, 
            ILogger logger)
        {
            _serverViewModelFactory = serverViewModelFactory;
            _settingsViewModel = settingsViewModel;

            Activator = viewModelActivator;
            this.logger = logger;

            _settingsViewModel.WhenAnyValue(x => x.ServerUpdate)
                .Where(x => x == true)
                .Subscribe(async x => await NavigateToServerAsync(), y => _settingsViewModel.ServerUpdate = false);

            this.WhenActivated(async (CompositeDisposable disposables) =>
            {
                await OnActivationAsync();
                Disposable
                    .Create(() => { /* handle deactivation */ })
                    .DisposeWith(disposables);
            });
        }

        private ServerViewModel GenerateNewServerViewModel()
        {
            return _serverViewModelFactory.GetNewServerViewModel(_settingsViewModel.GetSettings());
        }

        public async Task OnActivationAsync()
        {
            if (_settingsViewModel.IsServerConfigured())
            {
                await NavigateToServerAsync();
            }
            else
            {
                await NavigateToSettings();
            }
        }

        private async Task NavigateToServerAsync()
        {
            await Router.Navigate.Execute(GenerateNewServerViewModel());
        }

        public async Task NavigateToSettings()
        {
            await Router.Navigate.Execute(_settingsViewModel);
        }
    }
}