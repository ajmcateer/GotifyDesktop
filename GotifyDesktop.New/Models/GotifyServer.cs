using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.New.Models
{
    public class GotifyServer : ReactiveObject
    {
        string url;

        [JsonProperty]
        public string ID { get; set; }

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
        public string IconPath { get; set; }

        [JsonProperty]
        public string Url
        {
            get => url;
            set => this.RaiseAndSetIfChanged(ref url, value);
        }

        public GotifyServer()
        {
            ID = Guid.NewGuid().ToString();
        }

        public string GetHostPath()
        {
            return $"{Protocol}://{Url}:{Port}/{Path}";
        }
    }
}
