using GotifyDesktop.Exceptions;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Models;
using GotifyDesktop.Service;
using gotifySharp.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using static gotifySharp.Enums.ConnectionInfo;

namespace GotifyDesktop.ViewModels
{
    public class ServerViewModel : ViewModelBase, IRoutableViewModel, IActivatableViewModel
    {
        private bool firstActivation = true;
        public ViewModelActivator Activator { get; }

        private IGotifyServiceFactory _gotifyServiceFactory;
        private IGotifyService _gotifyService;
        private Dictionary<int, ObservableCollection<RxMessageModel>> _serverCache;

        public IScreen HostScreen { get; }
        ObservableCollection<RxApplicationModel> applications;
        RxApplicationModel selectedApplication;
        ObservableCollection<RxMessageModel> messageModels;

        AlertMessageViewModel alertMessageViewModel;
        SettingsViewModel _settingsViewModel;

        public ObservableCollection<RxMessageModel> MessageModels
        {
            get => messageModels;
            set => this.RaiseAndSetIfChanged(ref messageModels, value);
        }

        public RxApplicationModel SelectedItem
        {
            get => selectedApplication;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedApplication, value);
                //UpdateMessageDisplayAsync();
            }
        }

        public ObservableCollection<RxApplicationModel> Applications
        {
            get => applications;
            private set => this.RaiseAndSetIfChanged(ref applications, value);
        }

        public AlertMessageViewModel AlertMessageViewModel
        {
            get => alertMessageViewModel;
            set => this.RaiseAndSetIfChanged(ref alertMessageViewModel, value);
        }

        public string UrlPathSegment => throw new NotImplementedException();

        public ServerViewModel(IGotifyServiceFactory gotifyServiceFactory,
            SettingsViewModel settingsViewModel,
            IScreen screen)
        {
            _gotifyServiceFactory = gotifyServiceFactory;
            _serverCache = new Dictionary<int, ObservableCollection<RxMessageModel>>();
            _settingsViewModel = settingsViewModel;
            HostScreen = screen;
            Activator = new ViewModelActivator();
            AlertMessageViewModel = new AlertMessageViewModel();

            messageModels = new ObservableCollection<RxMessageModel>();
            applications = new ObservableCollection<RxApplicationModel>();

            this.WhenAnyValue(value => value.SelectedItem.Changed)
                .Where(value => value != null)
                .Subscribe(ValueTask => UpdateMessageDisplay());

            this.WhenAnyValue(value => value._settingsViewModel.ServerUpdate)
                .Where(value => value == true)
                .Subscribe(async ValueTask => await ReConfigureAsync(ValueTask));

            this.WhenActivated(async (CompositeDisposable disposables) =>
            {
                await OnActivationAsync();
                Disposable
                    .Create(() => { OnCloseAsync(); })
                    .DisposeWith(disposables);
            });
        }

        private async Task ReConfigureAsync(bool valueTask)
        {
            _gotifyService = _gotifyServiceFactory.CreateNewGotifyService(_settingsViewModel.GetSettings());
            await DoSync();
        }

        private void OnCloseAsync()
        {
            
        }

        private async Task OnActivationAsync()
        {
            if (firstActivation)
            {
                if (_settingsViewModel.IsServerConfigured())
                {
                    _gotifyService = _gotifyServiceFactory.CreateNewGotifyService(_settingsViewModel.GetSettings());
                    await DoSync();
                }
                else
                {
                    ShowSettings();
                }
            }
        }

        private void _gotifyService_OnReconnect(object sender, WebsocketReconnectStatus e)
        {
            AlertMessageViewModel.IsDisplayVisible = false;
        }

        private void _gotifyService_OnDisconnect(object sender, WebsocketDisconnectStatus e)
        {
            if(e != WebsocketDisconnectStatus.NoMessageReceived)
            {
                AlertMessageViewModel.IsDisplayVisible = true;
            }
        }

        private void SyncService_OnMessageRecieved(object sender, MessageModel e)
        {
            if (_serverCache.ContainsKey(e.Appid))
            {
                if(_serverCache[e.Appid].Count == 1)
                {
                    if(_serverCache[e.Appid][0].Id == -1)
                    {
                        _serverCache[e.Appid].RemoveAt(0);
                    }
                }
                _serverCache[e.Appid].Insert(0, new RxMessageModel(e));
            }
            if(Applications != null)
            {
                var app = Applications.Where(x => x.Id == e.Appid).FirstOrDefault();
                if(SelectedItem != null)
                {
                    if(SelectedItem.Id != app.Id)
                    {
                        app.HasAlert = true;
                    }
                }
                else
                {
                    app.HasAlert = true;
                }
            }
        }

        private void UpdateMessageDisplay()
        {
            if (!_serverCache.ContainsKey(SelectedItem.Id))
            {
                _serverCache[SelectedItem.Id] = new ObservableCollection<RxMessageModel>();
                _serverCache[SelectedItem.Id].Add(new RxMessageModel()
                {
                    Title = "Nothing to show",
                    DateString = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                    Message = "You haven't gotten any alerts yet",
                    Id = -1
                });
                MessageModels = _serverCache[SelectedItem.Id];
            }
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

        private async Task DoSync()
        {
            try
            {
                _gotifyService.OnDisconnect += _gotifyService_OnDisconnect;
                _gotifyService.OnMessage += SyncService_OnMessageRecieved;
                _gotifyService.OnReconnect += _gotifyService_OnReconnect;
                AlertMessageViewModel.Retry += AlertMessageViewModel_RetryAsync;
                _gotifyService.InitWebsocket();

                var results = await _gotifyService.GetApplications();
                if(results != null)
                {
                    foreach (var result in results)
                    {
                        Applications.Add(new RxApplicationModel(result));
                        _serverCache[result.id] = new ObservableCollection<RxMessageModel>();
                        _serverCache[result.id].Add(new RxMessageModel()
                        {
                            Title = "Nothing to show",
                            DateString = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                            Message = "You haven't gotten any alerts yet",
                            Id = -1
                        });
                    }
                    AlertMessageViewModel.IsDisplayVisible = false;
                    firstActivation = false;
                }
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
            HostScreen.Router.Navigate.Execute(_settingsViewModel);
        }
    }
}
