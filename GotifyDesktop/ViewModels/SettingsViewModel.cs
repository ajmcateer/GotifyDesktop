using GotifyDesktop.Interfaces;
using GotifyDesktop.Models;
using GotifyDesktop.Service;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class SettingsViewModel : ViewModelBase, IRoutableViewModel
    {

        // Reference to IScreen that owns the routable view model.
        public IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        public ServerInfo serverInfo;
        private ServerInfo newServerInfo;

        IServerPageInterface addServerViewModel;
        ISettingsPageInterface optionsViewModel;
        IDatabaseService databaseService;
        RoutingService routingService;

        public ServerInfo NewServerInfo
        {
            get => newServerInfo;
            set => this.RaiseAndSetIfChanged(ref newServerInfo, value);
        }

        public IDatabaseService DatabaseService
        {
            get => databaseService;
            set => this.RaiseAndSetIfChanged(ref databaseService, value);
        }

        public ISettingsPageInterface OptionsViewModel
        {
            get => optionsViewModel;
            set => this.RaiseAndSetIfChanged(ref optionsViewModel, value);
        }

        public IServerPageInterface AddServerViewModel
        {
            get => addServerViewModel;
            set => this.RaiseAndSetIfChanged(ref addServerViewModel, value);
        }

        public SettingsViewModel(AddServerViewModel addServerViewModel, OptionsViewModel optionsViewModel,
            IDatabaseService databaseService)
        {
            HostScreen = Locator.Current.GetService<IScreen>();

            AddServerViewModel = addServerViewModel;
            OptionsViewModel = optionsViewModel;
            DatabaseService = databaseService;

            //this.routingService = routingService;
        }

        public void SetupSettings(ServerInfo serverInfo)
        {
            addServerViewModel.SetServerInfo(serverInfo);
        }

        public void SetupNewSettings()
        {
            addServerViewModel.SetNewServer();
        }

        public async Task Apply()
        {
            NewServerInfo = AddServerViewModel.Save();

            databaseService.InsertServer(newServerInfo);
        }

        public async Task Save()
        {
            NewServerInfo = AddServerViewModel.Save();

            databaseService.InsertServer(newServerInfo);
            Cancel();
        }

        public async Task Cancel()
        {
            var test = Locator.Current.GetService<ICustomScreen>();
            _ = test.Router.NavigateBack.Execute();
        }
    }
}