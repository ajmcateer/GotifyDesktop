using GotifyDesktop.Interfaces;
using GotifyDesktop.Models;
using GotifyDesktop.Service;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class SettingsViewModel : ViewModelBase, IRoutableViewModel, IActivatableViewModel
    {

        // Reference to IScreen that owns the routable view model.
        public IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        public ServerInfo serverInfo;
        SettingsService _settingsService;

        bool _serverUpdate;

        AddServerViewModel addServerViewModel;
        ISettingsPageInterface optionsViewModel;

        public ViewModelActivator Activator { get; }

        public ISettingsPageInterface OptionsViewModel
        {
            get => optionsViewModel;
            set => this.RaiseAndSetIfChanged(ref optionsViewModel, value);
        }

        public AddServerViewModel AddServerViewModel
        {
            get => addServerViewModel;
            set => this.RaiseAndSetIfChanged(ref addServerViewModel, value);
        }        
        
        public bool ServerUpdate
        {
            get => _serverUpdate;
            set => this.RaiseAndSetIfChanged(ref _serverUpdate, value);
        }

        public SettingsViewModel(AddServerViewModel addServerViewModel, 
            OptionsViewModel optionsViewModel,
            SettingsService settingsService,
            ViewModelActivator viewModelActivator)
        {
            HostScreen = Locator.Current.GetService<IScreen>();
            AddServerViewModel = addServerViewModel;
            OptionsViewModel = optionsViewModel;
            _settingsService = settingsService;

            ServerUpdate = false;

            Activator = viewModelActivator;

            this.AddServerViewModel.WhenAnyValue(x => x.UpdatedServer)
                .Where(x => x != null)
                .Subscribe(x => SaveRx(x));

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                OnActivation();
                Disposable
                    .Create(() => { /* handle deactivation */ })
                    .DisposeWith(disposables);
            });
        }

        private void OnActivation()
        {
            if (_settingsService.IsServerConfigured())
            {
                AddServerViewModel.SetServerInfo(_settingsService.GetSettings());
            }
            else
            {
                AddServerViewModel.SetNewServer();
            }
        }

        internal ServerInfo GetSettings()
        {
            return _settingsService.GetSettings();
        }

        internal bool IsServerConfigured()
        {
            return _settingsService.IsServerConfigured();
        }

        public void SaveRx(ServerInfo serverInfo)
        {
            _settingsService.SaveSettings(serverInfo);
            ServerUpdate = true;
        }

        public async Task Back()
        {
            var test = Locator.Current.GetService<ICustomScreen>();
            await test.Router.NavigateBack.Execute();
        }
    }
}