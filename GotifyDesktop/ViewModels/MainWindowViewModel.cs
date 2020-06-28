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

namespace GotifyDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ILogger logger;
        
        BusyViewModel busyViewModel;

        ObservableCollection<ExtendedApplicationModel> applications;
        ExtendedApplicationModel selectedApplication;
        ObservableCollection<MessageModel> messageModels;
        AlertMessageViewModel alertMessageViewModel;
        SettingsViewModel settingsViewModel;

        IDatabaseService databaseService;
        ISyncService syncService;

        public BusyViewModel BusyViewModel
        {
            get => busyViewModel;
            set => this.RaiseAndSetIfChanged(ref busyViewModel, value);
        }

        public AlertMessageViewModel AlertMessageViewModel
        {
            get => alertMessageViewModel;
            set => this.RaiseAndSetIfChanged(ref alertMessageViewModel, value);
        }

        public SettingsViewModel SettingsViewModel
        {
            get => settingsViewModel;
            set => this.RaiseAndSetIfChanged(ref settingsViewModel, value);
        }

        public ObservableCollection<MessageModel> MessageModels
        {
            get => messageModels;
            set => this.RaiseAndSetIfChanged(ref messageModels, value);
        }

        public ExtendedApplicationModel SelectedItem
        {
            get => selectedApplication;
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedApplication, value);
                UpdateMessageDisplayAsync();
            }
        }

        public ObservableCollection<ExtendedApplicationModel> Applications
        {
            get => applications;
            private set => this.RaiseAndSetIfChanged(ref applications, value);
        }

        public MainWindowViewModel()
        {

        }

        public MainWindowViewModel(SettingsViewModel settingsViewModel, BusyViewModel busyViewModel, AlertMessageViewModel alertMessageViewModel, 
            IDatabaseService databaseService, ISyncService syncService, ILogger logger)
        {
            
            BusyViewModel = busyViewModel;
            AlertMessageViewModel = alertMessageViewModel;
            SettingsViewModel = settingsViewModel;

            this.databaseService = databaseService;
            this.syncService = syncService;
            this.logger = logger;

            syncService.ConnectionState += SyncService_ConnectionState;
            syncService.OnMessageRecieved += SyncService_OnMessageRecieved;
            AlertMessageViewModel.Retry += AlertMessageViewModel_RetryAsync;

            messageModels = new ObservableCollection<MessageModel>();
            applications = new ObservableCollection<ExtendedApplicationModel>();
        }

        private void SyncService_ConnectionState(object sender, ConnectionStatus e)
        {
            if (e == ConnectionStatus.Failed)
            {
                AlertMessageViewModel.IsDisplayVisible = true;
            }
            else if (e == ConnectionStatus.Successful)
            {
                AlertMessageViewModel.IsDisplayVisible = false;
            }
        }

        private async void AddServerViewModel_saveServerEventAsync()
        {
            Initialize(this, null);
        }

        private async void AlertMessageViewModel_RetryAsync(object sender, EventArgs e)
        {
            await DoSync();
        }

        private async Task DoSync()
        {
            try
            {
                syncService.InitWebsocket();
            }
            catch (SyncFailureException excp)
            {
                Console.WriteLine("SyncFailure");
                //_logger.Error(excp, "Application Sync Failed");
            }
            catch (Exception excep)
            {
                Console.WriteLine("Error!");
                //_logger.Error(excep, "Error!");
            }
        }

        private void SyncService_OnMessageRecieved(object sender, int e)
        {
            UpdateMessageDisplayAsync();
        }

        private async Task UpdateMessageDisplayAsync()
        {
            if(SelectedItem != null)
            {
                var res = await syncService.GetMessagesPerAppAsync(SelectedItem.id);
                res.Reverse();
                MessageModels = new ObservableCollection<MessageModel>(res);
            }
        }

        public async void Initialize(object sender, object parameter)
        {
            await SetupSeverAsync();
        }

        private async Task SetupSeverAsync()
        {
            var server = await GetServerInfo();
            await ConfigureGotify(server);
            Applications = await GetApplicationsAsync();
        }

        private async Task ConfigureGotify(ServerInfo server)
        {
            syncService.Configure(server.Url, server.Port, server.Username, server.Password, server.Path, server.Protocol);
        }

        private async Task<ServerInfo> GetServerInfo()
        {
            var server = databaseService.GetServer();

            if (server == null)
            {
                
            }

            return server;
        }

        private async Task<ObservableCollection<ExtendedApplicationModel>> GetApplicationsAsync()
        {
            try 
            { 
                BusyViewModel.Show();
                await DoSync();
                return new ObservableCollection<ExtendedApplicationModel>(await syncService.GetApplicationsAsync());
            }
            catch (Exception e)
            {
                return new ObservableCollection<ExtendedApplicationModel>();
            }
            finally
            {
                BusyViewModel.Close();
            }
        }

        public async Task LogoutAsync()
        {
            ThemeService.SetDarkTheme();
            await SettingsViewModel.ShowAsync();
            //var userResult = await Dialog.ShowDialogAsync(ButtonEnum.YesNo, "LogOut", "Are you sure you want to logout from the server!", Icon.Warning);
            //if(userResult == ButtonResult.Yes)
            //{
            //    //databaseService.ResetDB();
            //    Applications.Clear();
            //    MessageModels.Clear();

            //    await SetupSeverAsync();
            //}
        }
    }
}