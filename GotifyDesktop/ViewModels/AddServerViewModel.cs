﻿using Autofac;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Interfaces;
using GotifyDesktop.Models;
using GotifyDesktop.Service;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using ReactiveUI;
using Serilog;
//using SharpDX.Direct2D1;
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
        private ServerInfo updatedServer;
        private ServerInfo oldServer;

        private bool isVisible;
        private bool isSaveEnabled;
        private string url;
        private string port;
        private string username;
        private string path;
        private string password;
        private string clientname;
        private string selectedProtocol;

        public ServerInfo UpdatedServer
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

        public AddServerViewModel()
        {
            Protocols = new ObservableCollection<string>() { "Http", "Https"};
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
                var isConnGood = await GotifyService.TestConnectionAsync(Url, int.Parse(Port), Username, Password, Path, SelectedProtocol);
                if (isConnGood)
                {
                    await Dialog.ShowMessageAsync(ButtonEnum.Ok, "Success", "Server is reachable", Icon.Plus);
                    IsSaveEnabled = true;
                }
                else
                {
                    await Dialog.ShowMessageAsync(ButtonEnum.Ok, "Failure", "Server is not reachable", Icon.Error);
                    IsSaveEnabled = false;
                }
            }
            catch (Exception e)
            {
                await Dialog.ShowMessageAsync(ButtonEnum.Ok, "Failure", "Server is not reachable", Icon.Error);
                IsSaveEnabled = false;
            }
        }

        ServerInfo Save()
        {
            return new ServerInfo()
            {
                ClientName = this.ClientName,
                Password = this.Password,
                Path = this.Path,
                Port = int.Parse(this.Port),
                Protocol = this.SelectedProtocol,
                Url = this.Url,
                Username = this.Username
            };
        }

        public void SaveNew()
        {
            var tempServer = new ServerInfo(0, Url, int.Parse(Port), Username, Password, Path, SelectedProtocol, ClientName, "");
            UpdatedServer = tempServer;

            //saveServerEvent?.Invoke();
        }

        public virtual void SetServerInfo(ServerInfo serverInfo)
        {
            this.ClientName = serverInfo.ClientName;
            this.Password = serverInfo.Password;
            this.Path = serverInfo.Path;
            this.Port = serverInfo.Port.ToString();
            this.SelectedProtocol = serverInfo.Protocol;
            this.Url = serverInfo.Url;
            this.Username = serverInfo.Username;
        }

        public virtual void SetNewServer()
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
    }
}