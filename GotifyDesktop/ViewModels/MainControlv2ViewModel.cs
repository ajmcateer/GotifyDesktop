using Autofac;
using GotifyDesktop.Models;
using GotifyDesktop.Service;
using gotifySharp.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class MainControlv2ViewModel : ViewModelBase
    {
        IContainer container;
        ObservableCollection<ApplicationModel> applications;
        ApplicationModel selectedApplication;
        ObservableCollection<MessageModel> messageModels;
        AlertMessageViewModel alertMessageViewModel;

        DatabaseService databaseService;
        SyncService syncService;
        GotifyService gotifyService;
        ServerInfo gotifyServer;

        bool firstSync = false;

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

        public MainControlv2ViewModel(IContainer container)
        {
            this.container = container;
            messageModels = new ObservableCollection<MessageModel>();
            applications = new ObservableCollection<ApplicationModel>();
            AlertMessageViewModel = new AlertMessageViewModel();

            databaseService = container.Resolve<DatabaseService>();
            syncService = container.Resolve<SyncService>();
            gotifyService = container.Resolve<GotifyService>();
            gotifyService.ConnectionState += GotifyService_ConnectionState;
            syncService.OnMessageRecieved += SyncService_OnMessageRecieved;
            AlertMessageViewModel.Retry += AlertMessageViewModel_RetryAsync;
        }

        private async void AlertMessageViewModel_RetryAsync(object sender, EventArgs e)
        {
            try
            {
                if (!firstSync)
                {
                    await syncService.FullSyncAsync();
                    firstSync = true;
                }
                gotifyService.InitWebsocket();
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.Message);
            }
        }

        private void GotifyService_ConnectionState(object sender, ConnectionStatus e)
        {
            if (e == ConnectionStatus.Failed)
            {
                AlertMessageViewModel.IsDisplayVisible = true;
            }
            else if (e ==ConnectionStatus.Successful)
            {
                AlertMessageViewModel.IsDisplayVisible = false;
            }
        }

        private void SyncService_OnMessageRecieved(object sender, int e)
        {
            UpdateMessageDisplay();
        }

        private void UpdateMessageDisplay()
        {
            var res = databaseService.GetMessagesForApplication(SelectedItem.id);
            res.Reverse();
            MessageModels = new ObservableCollection<MessageModel>(res);
        }

        public async Task InitAsync()
        {
            try
            {
                gotifyServer = databaseService.GetServers()[0];
                gotifyService.Configure(gotifyServer.Url, gotifyServer.Port, gotifyServer.Username, gotifyServer.Password, gotifyServer.Path, gotifyServer.Protocol);
                AlertMessageViewModel_RetryAsync(this, null);
                List<ApplicationModel> results = databaseService.GetApplications();
                Applications = new ObservableCollection<ApplicationModel>(results);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}