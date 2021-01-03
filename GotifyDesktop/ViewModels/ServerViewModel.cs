using Avalonia.Controls;
using Avalonia.Controls.Selection;
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
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class ServerViewModel : ViewModelBase, IRoutableViewModel, IActivatableViewModel
    {
        private bool firstActivation = true;
        ILogger logger;
        public ViewModelActivator Activator { get; }
        private Dictionary<int, ObservableCollection<MessageModel>> _serverCache;

        public IScreen HostScreen { get; }
        ObservableCollection<ExtendedApplicationModel> applications;
        ExtendedApplicationModel selectedApplication;
        ObservableCollection<MessageModel> messageModels;

        BusyViewModel busyViewModel;
        AlertMessageViewModel alertMessageViewModel;

        ISyncService syncService;

        public ObservableCollection<MessageModel> MessageModels
        {
            get => messageModels;
            set => this.RaiseAndSetIfChanged(ref messageModels, value);
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
            _serverCache = new Dictionary<int, ObservableCollection<MessageModel>>();
            BusyViewModel = new BusyViewModel();
            this.syncService = syncService;
            Activator = new ViewModelActivator();            

            messageModels = new ObservableCollection<MessageModel>();
            applications = new ObservableCollection<ExtendedApplicationModel>();

            this.WhenActivated(async (CompositeDisposable disposables) =>
            {
                await OnActivationAsync();
                Disposable
                    .Create(() => { OnCloseAsync(); })
                    .DisposeWith(disposables);
            });
        }

        private void OnCloseAsync()
        {
            
        }

        private async Task OnActivationAsync()
        {
            if (firstActivation)
            {
                AlertMessageViewModel = new AlertMessageViewModel();
                syncService.OnMessageRecieved += SyncService_OnMessageRecieved;
                syncService.WebSocketConnectionState += SyncService_WebSocketConnectionState;
                AlertMessageViewModel.Retry += AlertMessageViewModel_RetryAsync;
                syncService.InitWebsocket();

                await DoSync();

                foreach (var app in Applications)
                {
                    _serverCache[app.Id] = new ObservableCollection<MessageModel>();
                }
                firstActivation = false;
            }
        }

        private void SyncService_WebSocketConnectionState(object sender, ConnectionStatus e)
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

        private void SyncService_OnMessageRecieved(object sender, MessageModel e)
        {
            if (_serverCache.ContainsKey(e.appid))
            {
                _serverCache[e.appid].Insert(0, e);
            }
            if(Applications != null)
            {
                var app = Applications.Where(x => x.Id == e.appid).FirstOrDefault();
                if(SelectedItem.Id != app.Id)
                {
                    app.HasAlert = true;
                }
            }
        }

        private async Task UpdateMessageDisplayAsync()
        {
            if (!_serverCache.ContainsKey(SelectedItem.Id))
            {
                _serverCache[SelectedItem.Id] = new ObservableCollection<MessageModel>();
                MessageModels = _serverCache[SelectedItem.Id];
            }
            //else if (_serverCache[SelectedItem.Id].Count == 0)
            //{
            //    try
            //    {
            //        var messages = await syncService.GetMessagesPerAppAsync(SelectedItem.Id);
            //        _serverCache[SelectedItem.Id] = new ObservableCollection<MessageModel>(messages);
            //        MessageModels = _serverCache[SelectedItem.Id];
            //    }
            //    catch(SyncFailureException SyncExp)
            //    {
            //        _serverCache[SelectedItem.Id] = new ObservableCollection<MessageModel>();
            //        MessageModels = _serverCache[SelectedItem.Id];
            //    }
            //}
            else
            {
                SelectedItem.HasAlert = false;
                MessageModels = _serverCache[SelectedItem.Id];
            }
        }

        private async void AlertMessageViewModel_RetryAsync(object sender, EventArgs e)
        {
            await DoSync();
        }

        public async Task GoToSettings()
        {
            var Screen = Locator.Current.GetService<ICustomScreen>();
            await Screen.NavigateToSettings();
        }

        private async Task DoSync()
        {
            try
            {
                Applications = new ObservableCollection<ExtendedApplicationModel>(await syncService.GetApplicationsAsync());
                AlertMessageViewModel.IsDisplayVisible = false;
            }
            catch (SyncFailureException excp)
            {
                Console.WriteLine("SyncFailure");
                AlertMessageViewModel.IsDisplayVisible = true;
                //_logger.Error(excp, "Application Sync Failed");
            }
            catch (Exception excep)
            {
                Console.WriteLine("Error!");
                AlertMessageViewModel.IsDisplayVisible = true;
                //_logger.Error(excep, "Error!");
            }
        }

        public void ShowSettings()
        {
            var HostScreen = Locator.Current.GetService<ICustomScreen>();
            HostScreen.NavigateToSettings();
        }
    }
}
