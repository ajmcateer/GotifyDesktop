
using Avalonia.Controls.Notifications;
using Avalonia.Media.Imaging;
using DesktopNotifications.FreeDesktop;
using DesktopNotifications.Windows;
using GotifyDesktop.New.Models;
using gotifySharp;
using gotifySharp.Api;
using gotifySharp.Models;
using gotifySharp.Responses;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;
using System.Linq;
using Websocket.Client.Models;
using static gotifySharp.Enums.ConnectionInfo;

namespace GotifyDesktop.New.ViewModels
{
    public class ServerViewModel : ViewModelBase, IRoutableViewModel, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }
        GotifySharp GotifySharp;

        private Dictionary<int, ObservableCollection<RxMessageModel>> AppCache;
        private Dictionary<int, Bitmap> ImageCache;

        public IScreen HostScreen { get; }
        ObservableCollection<RxApplicationModel> applications;
        bool isConnectionStatusVisable;
        RxApplicationModel selectedApplication;
        ObservableCollection<RxMessageModel> messageModels;

        Bitmap serverIcon;

        public Bitmap ServerIcon
        {
            get => serverIcon;
            set => this.RaiseAndSetIfChanged(ref serverIcon, value);
        }

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
            }
        }

        public bool IsConnectionStatusVisable
        {
            get => isConnectionStatusVisable;
            private set => this.RaiseAndSetIfChanged(ref isConnectionStatusVisable, value);
        }

        public ObservableCollection<RxApplicationModel> Applications
        {
            get => applications;
            private set => this.RaiseAndSetIfChanged(ref applications, value);
        }

        public string UrlPathSegment => throw new NotImplementedException();

        private DesktopNotifications.INotificationManager NotificationManager;

        public ServerViewModel(GotifySharp gotifySharp, 
            DesktopNotifications.INotificationManager notificationManager)
        {
            GotifySharp = gotifySharp;
            IsConnectionStatusVisable = false;
            AppCache = new Dictionary<int, ObservableCollection<RxMessageModel>>();
            ImageCache = new Dictionary<int, Bitmap>();
            Activator = new ViewModelActivator();

            NotificationManager = notificationManager;

            messageModels = new ObservableCollection<RxMessageModel>();
            applications = new ObservableCollection<RxApplicationModel>();
            
            this.WhenAnyValue(value => value.SelectedItem.Changed)
                .Where(value => value != null)
                .Subscribe(ValueTask => UpdateMessageDisplay());

            this.WhenActivated(async (CompositeDisposable disposables) =>
            {
                await OnActivationAsync();
                Disposable
                    .Create(() => { OnCloseAsync(); })
                    .DisposeWith(disposables);
            });
        }

        public async Task InitWebsocket()
        {
            await GotifySharp.Stream.InitWebSocketAsync();
            GotifySharp.Stream.WsClient.MessageReceived.Subscribe(msg => WsIncomingMessageAsync(msg));
            GotifySharp.Stream.WsClient.DisconnectionHappened.Subscribe(msg => Ws_OnClose(msg));
            GotifySharp.Stream.WsClient.ReconnectionHappened.Subscribe(msg => Ws_OnOpen(msg));
        }

        private void Ws_OnOpen(ReconnectionInfo msg)
        {
            IsConnectionStatusVisable = false;
        }

        private void Ws_OnClose(DisconnectionInfo msg)
        {
            IsConnectionStatusVisable = true;
        }

        private DesktopNotifications.Notification CreateNotification(string title, string body)
        {
            return new DesktopNotifications.Notification
            {
                Title = title,
                Body = body,
                Buttons =
                {
                    ("OK", "okay")
                }
            };
        }

        private async Task WsIncomingMessageAsync(ResponseMessage msg)
        {
            var message = JsonSerializer.Deserialize<RxMessageModel>(msg.Text, new JsonSerializerOptions { 
                PropertyNameCaseInsensitive = true
            });

            try
            {
                await NotificationManager.ShowNotification(CreateNotification(message.Title, message.Message));
            }
            catch(Exception ex)
            {
                Console.Out.WriteLine("Exception");
                Console.Out.WriteLine(ex.Message);
            }

            if (AppCache.ContainsKey(message.Appid))
            {
                ProcessMessage(message);
            }
            else
            {
                var result = await GotifySharp.Application.GetApplicationsAsync();
                var newApp = result.ApplicationResponse.Where(x => x.id == message.Appid).FirstOrDefault();
                if(newApp != null)
                {
                    await GetApplication(newApp);
                    ProcessMessage(message);
                }
            }
        }

        private void ProcessMessage(RxMessageModel message)
        {
            message.Cover = ImageCache[message.Appid];
            AppCache[message.Appid].Insert(0, message);
            if (SelectedItem.Id != message.Appid)
            {
                Applications.Where(x => x.Id == message.Appid).FirstOrDefault().HasAlert = true;
            }
        }

        private void OnCloseAsync()
        {
            
        }

        private async Task OnActivationAsync()
        {
            int retries = 0;
            while (true)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Trying");
                    await DoSync();
                    IsConnectionStatusVisable = false;
                    await Task.Delay(5000);

                    return;
                }
                catch when (retries == 0) { }
            }
        }

        private void UpdateMessageDisplay()
        {
            SelectedItem.HasAlert = false;
            MessageModels = AppCache[SelectedItem.Id];
        }

        private async Task DoSync()
        {
            Applications.Clear();

            try
            {
                var result = await GotifySharp.Application.GetApplicationsAsync();
                foreach (var res in result.ApplicationResponse)
                {
                    await GetApplication(res);
                }

                SelectedItem = Applications[0];
                await InitWebsocket();
            }
            catch (HttpRequestException ex)
            {
                IsConnectionStatusVisable = true;
                throw;
            }
        }

        private async Task GetApplication(ApplicationModel applications)
        {
            try
            {
                var app = new RxApplicationModel(applications);
                var image = await GotifySharp.Application.DownloadApplicationPhoto(app.Image);
                ImageCache.Add(applications.id, Bitmap.DecodeToWidth(image, 64));
                Applications.Add(app);
                AppCache.Add(app.Id, new ObservableCollection<RxMessageModel>());
            }
            catch (Exception ex)
            {

            }
        }
    }
}
