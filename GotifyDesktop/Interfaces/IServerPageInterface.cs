using GotifyDesktop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Interfaces
{
    public interface IServerPageInterface
    {
        ServerInfo UpdatedServer {get;set;}
        public ServerInfo Save();
        public void SetServerInfo(ServerInfo serverInfo);
        public void SetNewServer();
    }
}
