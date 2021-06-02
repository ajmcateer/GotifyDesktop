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
        public IScreen HostScreen { get; set; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        ISettingsService _settingsService;

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
            ISettingsService settingsService)
        {
            AddServerViewModel = addServerViewModel;
            OptionsViewModel = optionsViewModel;
            _settingsService = settingsService;

            ServerUpdate = false;

            Activator = new ViewModelActivator();

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
                AddServerViewModel.SetServerInfo(GetSettings());
            }
            else
            {
                AddServerViewModel.SetNewServer();
            }
        }

        public ServerInfo GetSettings()
        {
            return _settingsService.GetSettings();
        }

        public bool IsServerConfigured()
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
            await HostScreen.Router.NavigateBack.Execute();
        }
    }
}