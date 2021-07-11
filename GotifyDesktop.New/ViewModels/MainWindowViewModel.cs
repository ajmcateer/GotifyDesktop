using Avalonia.Controls.Notifications;
using GotifyDesktop.New.External;
using GotifyDesktop.New.Models;
using GotifyDesktop.New.Services;
using GotifyDesktop.New.Settings;
using gotifySharp;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Websocket.Client.Exceptions;

namespace GotifyDesktop.New.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IScreen, IActivatableViewModel
    {
        private ObservableCollection<GotifyServer> serverList;
        private GotifyServer server;
        IRoutableViewModel selectedViewModel;

        Dictionary<string, IRoutableViewModel> ServerCache;

        public ObservableCollection<GotifyServer> ServerList
        {
            get => serverList;
            set => this.RaiseAndSetIfChanged(ref serverList, value);
        }

        public IRoutableViewModel SelectedViewModel
        {
            get => selectedViewModel;
            set => this.RaiseAndSetIfChanged(ref selectedViewModel, value);
        }

        public GotifyServer SelectedServer
        {
            get => server;
            set => this.RaiseAndSetIfChanged(ref server, value);
        }

        public ViewModelActivator Activator { get; }
        AddServerViewModel _addServerViewModel;
        public RoutingState Router { get; }
        ISettingsService _settingService;
        private readonly DesktopNotifications.INotificationManager NotificationManager;

        INotificationServerFactory NotificationServerFactory;

        public MainWindowViewModel(AddServerViewModel addServerViewModel,
            ISettingsService settingService,
            INotificationServerFactory notificationServerFactory,
            RoutingState routingState)
        {
            Console.Out.WriteLine("OUT");
            System.Diagnostics.Debug.WriteLine("DEBUG");

            _settingService = settingService;
            _addServerViewModel = addServerViewModel;
            Router = routingState;
            Activator = new ViewModelActivator();
            NotificationServerFactory = notificationServerFactory;

            ServerCache = new Dictionary<string, IRoutableViewModel>();

            ServerList = new ObservableCollection<GotifyServer>();

            _addServerViewModel.WhenAnyValue(value => value.Server)
                .Where(value => value != null)
                .Subscribe(async server => await NewServerAsync(server));

            this.WhenAnyValue(x => x.SelectedViewModel)
                             .Where(x => x != null)
                             .Subscribe(async server => await ChangeView());

            this.WhenAnyValue(x => x.SelectedServer)
                 .Where(x => x != null)
                 .Subscribe(async server => await SwitchToSelectedServer());

            this.WhenActivated(async (CompositeDisposable disposables) =>
            {
                await OnActivationAsync();
                Disposable
                    .Create(() => { /* handle deactivation */ })
                    .DisposeWith(disposables);
            });
        }

        private async Task ChangeView()
        {
            await Router.NavigateAndReset.Execute(SelectedViewModel);
        }

        public async Task SwitchToSelectedServer()
        {
            SelectedViewModel = ServerCache[SelectedServer.ID];
        }

        public async Task SignOut()
        {
            ServerList.Remove(SelectedServer);
            SelectedViewModel = _addServerViewModel;
            _settingService.DeleteServer();
        }

        private async Task NewServerAsync(GotifyServer server)
        {

            try
            {
                var serviewViewModel = NotificationServerFactory.GenerateNewView(server);

                ServerCache.Add(server.ID, serviewViewModel);
                AddServerToList(server);
                SelectedViewModel = ServerCache[server.ID];
            }
            catch (WebsocketException ex)
            {
                Console.Out.WriteLine("Websocket Exception");
                Console.Out.WriteLine(ex.Message);

                System.Diagnostics.Debug.WriteLine("Websocket Exception");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("Exception");
                Console.Out.WriteLine(ex.Message);

                System.Diagnostics.Debug.WriteLine("Exception");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }


        }

        private void AddServerToList(GotifyServer server)
        {
            ServerList.Add(server);
        }

        private async Task OnActivationAsync()
        {
            if (_settingService.DoesSettingsExist())
            {
                GotifyServer server = _settingService.GetSettings();
                await NewServerAsync(server);
            }
            else
            {
                SelectedViewModel = _addServerViewModel;
            }
        }
    }
}
