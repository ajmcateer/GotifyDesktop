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
        bool firstSync = true;

        ILogger _logger;
        AddServerViewModel addServerViewModel;
        BusyViewModel busyViewModel;

        ObservableCollection<ApplicationModel> applications;
        ApplicationModel selectedApplication;
        ObservableCollection<MessageModel> messageModels;
        AlertMessageViewModel alertMessageViewModel;
        IContainer container;

        DatabaseService databaseService;
        SyncService syncService;

        ServerInfo gotifyServer;

        public AddServerViewModel AddServerViewModel
        {
            get => addServerViewModel;
            set => this.RaiseAndSetIfChanged(ref addServerViewModel, value);
        }

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

        public ObservableCollection<MessageModel> MessageModels
        {
            get => messageModels;
            set => this.RaiseAndSetIfChanged(ref messageModels, value);
        }

        public ApplicationModel SelectedItem
        {
            get => selectedApplication;
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedApplication, value);
                UpdateMessageDisplay();
            }
        }

        public ObservableCollection<ApplicationModel> Applications
        {
            get => applications;
            private set => this.RaiseAndSetIfChanged(ref applications, value);
        }

        public MainWindowViewModel()
        {

        }

        public MainWindowViewModel(IContainer container)
        {
            this.container = container;

            syncService = container.Resolve<SyncService>();

            this.container = container;
            AddServerViewModel = new AddServerViewModel(container);
            BusyViewModel = new BusyViewModel();
            messageModels = new ObservableCollection<MessageModel>();
            applications = new ObservableCollection<ApplicationModel>();
            AlertMessageViewModel = new AlertMessageViewModel();

            databaseService = container.Resolve<DatabaseService>();
            syncService = container.Resolve<SyncService>();
            syncService.ConnectionState += SyncService_ConnectionState;
            syncService.OnMessageRecieved += SyncService_OnMessageRecieved;
            AlertMessageViewModel.Retry += AlertMessageViewModel_RetryAsync;
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
                if (firstSync)
                {
                    await syncService.FullSyncAsync();
                    firstSync = false;
                }
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
            UpdateMessageDisplay();
        }

        private void UpdateMessageDisplay()
        {
            if(SelectedItem != null)
            {
                var res = databaseService.GetMessagesForApplication(SelectedItem.id);
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
            await syncService.Configure(server.Url, server.Port, server.Username, server.Password, server.Path, server.Protocol);
        }

        private async Task<ServerInfo> GetServerInfo()
        {
            var server = databaseService.GetServer();

            if (server == null)
            {
                return await AddServerViewModel.ShowAsync();
            }

            return server;
        }

        private async Task<ObservableCollection<ApplicationModel>> GetApplicationsAsync()
        {
            try 
            { 
                BusyViewModel.Show();
                await DoSync();
                return new ObservableCollection<ApplicationModel>(databaseService.GetApplications());
            }
            catch (Exception e)
            {
                return new ObservableCollection<ApplicationModel>();
            }
            finally
            {
                BusyViewModel.Close();
            }
        }

        public async Task LogoutAsync()
        {
            var userResult = await Dialog.ShowDialogAsync(ButtonEnum.YesNo, "LogOut", "Are you sure you want to logout from the server!", Icon.Warning);
            if(userResult == ButtonResult.Yes)
            {
                databaseService.ResetDB();
                Applications.Clear();
                MessageModels.Clear();
                
                firstSync = true;

                await SetupSeverAsync();
            }
        }
    }
}