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
    internal class AddServerViewModel : ViewModelBase
    {
        public string Url { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public string SelectedProtocol { get; set; }

        ObservableCollection<string> protocols;

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
            SelectedProtocol = "Http";
        }

        public void Save()
        {
            Settings.Password = Password;
            Settings.Port = Int32.Parse(Port);
            Settings.Url = Url;
            Settings.User = Username;
            Settings.Protocol = SelectedProtocol;
            Settings.Path = Path;

            var gotifyService = container.Resolve<GotifyService>();
            //gotifyService.Configure(Url, int.Parse(Port), Username, Password, Path, SelectedProtocol);

            var databaseService = container.Resolve<DatabaseService>();
            var serverInfo = new ServerInfo(0, Url, Int32.Parse(Port), Username, Password, Path, SelectedProtocol);
            databaseService.InsertServer(serverInfo);

            saveServerEvent?.Invoke();
        }

        public async Task CheckConnection()
        {
            //TODO Fix caching issue
            GotifyService gotifyService = container.ResolveNamed<GotifyService>("TestService");
            var isConnGood = await gotifyService.TestConnectionAsync(Url, int.Parse(Port), Username, Password, Path, SelectedProtocol);
            if (isConnGood)
            {
                await Dialog.ShowMessageAsync(ButtonEnum.Ok, "Success", "Server is reachable", Icon.Success);
            }
            else
            {
                await Dialog.ShowMessageAsync(ButtonEnum.Ok, "Failure", "Server is not reachable", Icon.Error);
            }
        }
    }
}
