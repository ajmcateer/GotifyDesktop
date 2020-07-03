using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Models
{
    public class ServerInfo
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Protocol { get; set; }
        public string Path { get; set; }
        public string ClientName { get; set; }

        public ServerInfo()
        {

        }

        public ServerInfo(int ID, string Url, int Port, string Username, string Password, string Path, string Protocol, string ClientName)
        {
            this.ID = ID;
            this.Url = Url;
            this.Port = Port;
            this.Username = Username;
            this.Password = Password;
            this.Protocol = Protocol;
            this.Path = Path;
            this.ClientName = ClientName;
        }
    }
}