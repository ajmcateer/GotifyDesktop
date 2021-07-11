using GotifyDesktop.New.Models;
using GotifyDesktop.New.Settings;
using gotifySharp;
using MessageBox.Avalonia.Enums;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GotifyDesktop.New.ViewModels
{
    public class AddServerViewModel : ViewModelBase, IRoutableViewModel
    {
        ObservableCollection<string> protocols;
        private GotifyServer updatedServer;
        private string clientToken;

        private bool isSaveEnabled;
        private string url;
        private string username;
        private string password;
        private string clientName;
        private string serverName;
        private string selectedProtocol;

        public GotifyServer Server
        {
            get => updatedServer;
            set => this.RaiseAndSetIfChanged(ref updatedServer, value);
        }

        public GotifyServer UpdatedServer
        {
            get => updatedServer;
            set => this.RaiseAndSetIfChanged(ref updatedServer, value);
        }

        public string SelectedProtocol
        {
            get => selectedProtocol;
            set => this.RaiseAndSetIfChanged(ref selectedProtocol, value);
        }

        public string ClientName
        {
            get => clientName;
            set => this.RaiseAndSetIfChanged(ref clientName, value);
        }

        public string ServerName
        {
            get => serverName;
            set => this.RaiseAndSetIfChanged(ref serverName, value);
        }

        public string Url
        {
            get => url;
            set => this.RaiseAndSetIfChanged(ref url, value);
        }

        public string Username
        {
            get => username;
            set => this.RaiseAndSetIfChanged(ref username, value);
        }

        public string Password
        {
            get => password;
            set => this.RaiseAndSetIfChanged(ref password, value);
        }

        public bool IsSaveEnabled
        {
            get => isSaveEnabled;
            set => this.RaiseAndSetIfChanged(ref isSaveEnabled, value);
        }

        public ObservableCollection<string> Protocols
        {
            get => protocols;
            set => this.RaiseAndSetIfChanged(ref protocols, value);
        }

        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen => throw new NotImplementedException();

        ISettingsService _settingService;

        public AddServerViewModel(ISettingsService settingService)
        {
            Protocols = new ObservableCollection<string>() { "Http", "Https" };
            _settingService = settingService;

            IsSaveEnabled = false;
        }

        private string GenerateClientName()
        {
            string epoch = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            return "GotifyDesktop-" + epoch.Substring(epoch.Length - 8);
        }

        public async Task CheckConnection()
        {
            try
            {
                var clientResponse = await GotifySharp.GetClientTokenAsync(Url, ClientName, Username, Password);
                if (clientResponse.Success)
                {
                    clientToken = clientResponse.ClientModel.Token;
                    var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                        .GetMessageBoxStandardWindow("Success", "Server is reachable", ButtonEnum.Ok, Icon.Success);
                    await messageBoxStandardWindow.Show();

                    IsSaveEnabled = true;
                }
                else
                {
                    var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                        .GetMessageBoxStandardWindow("Failure", "Server is not reachable", ButtonEnum.Ok, Icon.Error);
                    await messageBoxStandardWindow.Show();

                    IsSaveEnabled = false;
                }
            }
            catch (Exception e)
            {
                var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                        .GetMessageBoxStandardWindow("Failure", "Server is not reachable", ButtonEnum.Ok, Icon.Error);
                await messageBoxStandardWindow.Show();

                IsSaveEnabled = false;
            }
        }

        GotifyServer GetServerFromUi()
        {
            return new GotifyServer()
            {
                ServerName = ServerName,
                ClientName = ClientName,
                Password = Password,
                ClientToken = clientToken,
                Url = Url,
                Username = Username,
                IconPath = "/Assests/defaultapp.png"
            };
        }

        public async Task SaveNewAsync()
        {
            GotifyServer newServer = GetServerFromUi();
            _settingService.SaveSettings(newServer);
            Server = newServer;
        }
    }
}
