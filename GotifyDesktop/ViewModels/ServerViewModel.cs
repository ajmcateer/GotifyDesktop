using Avalonia.Controls;
using GotifyDesktop.Exceptions;
using GotifyDesktop.Interfaces;
using GotifyDesktop.Models;
using GotifyDesktop.Service;
using gotifySharp.Models;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class ServerViewModel : ViewModelBase, IRoutableViewModel
    {
        ServerInfo currentServer;

        ILogger logger;

        public IScreen HostScreen { get; }
        ObservableCollection<ExtendedApplicationModel> applications;
        ExtendedApplicationModel selectedApplication;
        ISelectionModel selectedObject;
        ObservableCollection<MessageModel> messageModels;

        BusyViewModel busyViewModel;
        AlertMessageViewModel alertMessageViewModel;

        ISyncService syncService;

        public ServerInfo CurrentServer
        {
            get => currentServer;
            set => this.RaiseAndSetIfChanged(ref currentServer, value);
        }

        public ObservableCollection<MessageModel> MessageModels
        {
            get => messageModels;
            set => this.RaiseAndSetIfChanged(ref messageModels, value);
        }

        public ISelectionModel Selection
        {
            get => selectedObject;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedObject, value);
            }
        }

        public ExtendedApplicationModel SelectedItem
        {
            get => selectedApplication;
            set
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

        public AlertMessageViewModel AlertMessageViewModel
        {
            get => alertMessageViewModel;
            set => this.RaiseAndSetIfChanged(ref alertMessageViewModel, value);
        }

        public BusyViewModel BusyViewModel
        {
            get => busyViewModel;
            set => this.RaiseAndSetIfChanged(ref busyViewModel, value);
        }

        public string UrlPathSegment => throw new NotImplementedException();

        public ServerViewModel(ISyncService syncService)
        {
            HostScreen = Locator.Current.GetService<IScreen>();
            BusyViewModel = new BusyViewModel();
            this.syncService = syncService;

            AlertMessageViewModel = new AlertMessageViewModel();
            syncService.ConnectionState += SyncService_ConnectionState;
            syncService.OnMessageRecieved += SyncService_OnMessageRecieved;
            AlertMessageViewModel.Retry += AlertMessageViewModel_RetryAsync;

            messageModels = new ObservableCollection<MessageModel>();
            applications = new ObservableCollection<ExtendedApplicationModel>();
        }

        private async void SyncService_OnMessageRecieved(object sender, int e)
        {
            await UpdateMessageDisplayAsync();
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

        private async Task UpdateMessageDisplayAsync()
        {
            if (SelectedItem != null)
            {
                var res = await syncService.GetMessagesPerAppAsync(SelectedItem.id);
                MessageModels = new ObservableCollection<MessageModel>(res);
            }
        }

        private async void AlertMessageViewModel_RetryAsync(object sender, EventArgs e)
        {
            await DoSync();
        }

        public async Task SetupSeverAsync(ServerInfo server)
        {
            currentServer = server;
            await ConfigureGotify(currentServer);
            Applications = await GetApplicationsAsync();
        }

        private async Task DoSync()
        {
            try
            {
                //syncService.InitWebsocket();
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

        private async Task ConfigureGotify(ServerInfo server)
        {
            if(server != null)
            {
                syncService.Configure(server.Url, server.Port, server.Username, server.Password, server.Path, server.Protocol);
            }
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

        public void Apply(object parameter)
        {

        }

        public void ShowSettings()
        {
            var HostScreen = Locator.Current.GetService<ICustomScreen>();
            HostScreen.NavigateToSettings();
        }
    }
}
