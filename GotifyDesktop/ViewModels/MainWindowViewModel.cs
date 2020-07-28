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

namespace GotifyDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, ICustomScreen
    {
        ILogger logger;
        
        ServerInfo currentServer;

        SettingsViewModel settingsViewModel;
        ServerViewModel serverViewModel;
        ObservableCollection<ServerViewModel> serverViewModels;

        IDatabaseService databaseService;
        ISyncService syncService;

        public RoutingState Router { get; } = new RoutingState();

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

        public MainWindowViewModel(SettingsViewModel settingsViewModel, ServerViewModel serverViewModel, IDatabaseService databaseService, 
            ISyncService syncService, ILogger logger)
        {
            ServerViewModels = new ObservableCollection<ServerViewModel>();
            SettingsViewModel = settingsViewModel;
            this.serverViewModel = serverViewModel;
            

            this.databaseService = databaseService;
            this.syncService = syncService;
            this.logger = logger;

            this.WhenAnyValue(x => x.SettingsViewModel.NewServerInfo)
                .Where(x => x != null)
                .Subscribe(x => testAsync(x));
        }

        private async Task SetupServerAsync()
        {
            await serverViewModel.SetupSeverAsync(currentServer);
        }

        private async void testAsync(ServerInfo ser)
        {
            currentServer = ser;
            await SetupServerAsync();
            ServerViewModels.Add(this.serverViewModel);
        }

        private async Task GetServerAsync()
        {
            await Router.Navigate.Execute(serverViewModel);
            currentServer = GetServerFromDb();
            if (currentServer == null)
            {
                GetServerFromUser();
            }
            else
            {
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

        public void NavigateToSettings()
        {
            SettingsViewModel.SetupSettings(currentServer);
            Router.Navigate.Execute(SettingsViewModel);
        }
    }
}