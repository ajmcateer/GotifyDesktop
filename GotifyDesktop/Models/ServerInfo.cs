using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Models
{
    public class ServerInfo : ReactiveObject
    {
        string url;

        [JsonProperty]
        public int ID { get; set; }

        //public string Url { get; set; }
        [JsonProperty]
        public int Port { get; set; }

        [JsonProperty]
        public string Username { get; set; }

        [JsonProperty]
        public string Password { get; set; }

        [JsonProperty]
        public string Protocol { get; set; }

        [JsonProperty]
        public string Path { get; set; }

        [JsonProperty]
        public string ClientName { get; set; }
        [JsonProperty]
        public string ClientToken { get; set; }

        [JsonProperty]
        public string ServerName { get; set; }

        [JsonProperty]
        public string Url
        {
            get => url;
            set => this.RaiseAndSetIfChanged(ref url, value);
        }

        public ServerInfo(string Url, int Port, string Username, string Password, string Path, string Protocol, string ClientName, string ServerName)
        {
            this.ID = ID;
            this.Url = Url;
            this.Port = Port;
            this.Username = Username;
            this.Password = Password;
            this.Protocol = Protocol;
            this.Path = Path;
            this.ClientName = ClientName;
            this.ServerName = ServerName;
        }

        public ServerInfo()
        {

        }

        public string GetHostPath()
        {
            return $"{Protocol}://{Url}:{Port}/{Path}";
        }
    }
}