using Autofac;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Models;
using GotifyDesktop.Service;
using gotifySharp.Models;
using MessageBox.Avalonia.Enums;
using Microsoft.EntityFrameworkCore.Internal;
using Serilog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GotifyDesktop.Exceptions;
using System.Reactive.Linq;
using Avalonia.Collections;
using GotifyDesktop.Interfaces;
using Avalonia.Controls;

namespace GotifyDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, ICustomScreen
    {
        ILogger logger;

        ObservableCollection<ServerInfo> _server;
        ServerInfo _selectedServer;

        SettingsViewModel settingsViewModel;
        ServerViewModel serverViewModel;
        ObservableCollection<ServerViewModel> serverViewModels;

        IDatabaseService databaseService;

        public RoutingState Router { get; } = new RoutingState();

        public ServerInfo SelectedServer
        {
            get => _selectedServer;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedServer, value);
            }
        }

        public ObservableCollection<ServerInfo> Servers
        {
            get => _server;
            set => this.RaiseAndSetIfChanged(ref _server, value);
        }

        public SettingsViewModel SettingsViewModel
        {
            get => settingsViewModel;
            set => this.RaiseAndSetIfChanged(ref settingsViewModel, value);
        }

        public ObservableCollection<ServerViewModel> ServerViewModels
        {
            get => serverViewModels;
            set => this.RaiseAndSetIfChanged(ref serverViewModels, value);
        }

        public MainWindowViewModel(SettingsViewModel settingsViewModel, ServerViewModel serverViewModel, IDatabaseService databaseService, ILogger logger)
        {
            ServerViewModels = new ObservableCollection<ServerViewModel>();
            SettingsViewModel = settingsViewModel;
            this.serverViewModel = serverViewModel;

            Servers = new ObservableCollection<ServerInfo>();
            this.databaseService = databaseService;
            this.logger = logger;

            this.WhenAnyValue(x => x.SettingsViewModel.AddServerViewModel.UpdatedServer)
                .Where(x => x != null)
                .Subscribe(x => testAsync(x));

            this.WhenAnyValue(x => x.SelectedServer)
                .Where(x => x != null)
                .Subscribe(x => ShowServerAsync(x));
        }

        private async Task ShowServerAsync(ServerInfo s)
        {
            await NavigateToServerAsync();
        }

        private async Task SetupServerAsync()
        {
            await serverViewModel.SetupSeverAsync(Servers[0]);
        }

        private async void testAsync(ServerInfo ser)
        {
            if(Servers.Count == 0)
            {
                Servers.Add(ser);                
            }
            else
            {
                Servers[0] = ser;
            }
            databaseService.UpdateServer(ser);
            await SetupServerAsync();
            ServerViewModels.Add(this.serverViewModel);
            await Router.Navigate.Execute(serverViewModel);
        }

        private async Task GetServerAsync()
        {
            await NavigateToServerAsync();
            var currentServer = GetServerFromDb();
            if (currentServer == null)
            {
                GetServerFromUser();
            }
            else
            {
                Servers.Add(currentServer);
                await SetupServerAsync();
                ServerViewModels.Add(this.serverViewModel);
            }
        }

        private ServerInfo GetServerFromDb()
        {
            return databaseService.GetServer();
        }

        private void GetServerFromUser()
        {
            settingsViewModel.SetupNewSettings();
            Router.Navigate.Execute(SettingsViewModel);
        }

        public void SetTheme()
        {
            ThemeService.SetSystemTheme();
        }

        public async void Initialize(object sender, object parameter)
        {
            //SetTheme();
            await GetServerAsync();
        }

        private async Task NavigateToServerAsync()
        {
            await Router.Navigate.Execute(serverViewModel);
        }

        public void NavigateToSettings()
        {
            SelectedServer = null;
            SettingsViewModel.SetupSettings(Servers[0]);
            Router.Navigate.Execute(SettingsViewModel);
        }
    }
}