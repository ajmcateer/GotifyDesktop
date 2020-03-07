using Autofac;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Models;
using GotifyDesktop.Service;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class AddServerViewModel : ViewModelBase
    {
        TaskCompletionSource<ServerInfo> taskCompletionSource;
        ObservableCollection<string> protocols;
        ServerInfo serverInfo;

        private bool isVisible;
        private bool isSaveEnabled;
        private string url;
        private string port;
        private string username;
        private string path;
        private string password;
        private string clientname;
        private string selectedProtocol;

        public string SelectedProtocol
        {
            get => selectedProtocol;
            set => this.RaiseAndSetIfChanged(ref selectedProtocol, value);
        }

        public string ClientName
        {
            get => clientname;
            set => this.RaiseAndSetIfChanged(ref clientname, value);
        }

        public string Path
        {
            get => path;
            set => this.RaiseAndSetIfChanged(ref path, value);
        }

        public string Url
        {
            get => url;
            set => this.RaiseAndSetIfChanged(ref url, value);
        }
               
        public string Port
        {
            get => port;
            set => this.RaiseAndSetIfChanged(ref port, value);
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

        public bool IsVisible
        {
            get => isVisible;
            set => this.RaiseAndSetIfChanged(ref isVisible, value);
        }

        public ObservableCollection<string> Protocols
        {
            get => protocols;
            set => this.RaiseAndSetIfChanged(ref protocols, value);
        }

        public delegate void SaveServer();

        //declare event of type delegate
        public event SaveServer saveServerEvent;

        IContainer container;

        public AddServerViewModel(IContainer container)
        {
            this.container = container;
            Protocols = new ObservableCollection<string>() { "Http", "Https"};
            ResetView();
        }

        public AddServerViewModel(IContainer container, ServerInfo serverInfo) : this(container)
        {
            this.serverInfo = serverInfo;
            Username = serverInfo.Username;
            Password = serverInfo.Password;
            Path = serverInfo.Path;
            Port = serverInfo.Port.ToString();
            Url = serverInfo.Url;
            SelectedProtocol = serverInfo.Protocol;
        }

        private void ResetView()
        {
            Path = "/";
            IsSaveEnabled = false;
            ClientName = GenerateClientName();
            SelectedProtocol = "Http";
            Url = String.Empty;
            Port = String.Empty;
            Username = String.Empty;
            Password = String.Empty;
        }

        private string GenerateClientName()
        {
            string epoch = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            return "GotifyDesktop-" + epoch.Substring(epoch.Length - 8);
        }

        public async Task<ServerInfo> ShowAsync()
        {
            ResetView();
            IsVisible = true;
            taskCompletionSource = new TaskCompletionSource<ServerInfo>();
            
            var result = await taskCompletionSource.Task;

            IsVisible = false;
            return result;
        }

        public void Save()
        {

            var gotifyService = container.Resolve<GotifyService>();
            //gotifyService.Configure(Url, int.Parse(Port), Username, Password, Path, SelectedProtocol);

            var databaseService = container.Resolve<DatabaseService>();
            var serverInfo = new ServerInfo(0, Url, Int32.Parse(Port), Username, Password, Path, SelectedProtocol, ClientName);
            databaseService.InsertServer(serverInfo);

            taskCompletionSource.SetResult(serverInfo);
            //saveServerEvent?.Invoke();
        }

        public void Close()
        {
            Program.window.Close();
        }

        public async Task CheckConnection()
        {
            GotifyService gotifyService = container.ResolveNamed<GotifyService>("TestService");
            try
            {
                var isConnGood = await gotifyService.TestConnectionAsync(Url, int.Parse(Port), Username, Password, Path, SelectedProtocol);
                if (isConnGood)
                {
                    await Dialog.ShowMessageAsync(ButtonEnum.Ok, "Success", "Server is reachable", Icon.Success);
                    IsSaveEnabled = true;
                }
                else
                {
                    await Dialog.ShowMessageAsync(ButtonEnum.Ok, "Failure", "Server is not reachable", Icon.Error);
                    IsSaveEnabled = false;
                }
            }
            catch(Exception e)
            {
                await Dialog.ShowMessageAsync(ButtonEnum.Ok, "Failure", "Server is not reachable", Icon.Error);
                IsSaveEnabled = false;
            }
        }
    }
}