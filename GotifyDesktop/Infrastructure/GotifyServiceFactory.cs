using GotifyDesktop.Models;
using GotifyDesktop.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Infrastructure
{
    public class GotifyServiceFactory : IGotifyServiceFactory
    {
        public IGotifyService CreateNewGotifyService(ServerInfo serverInfo, ILogger ilogger)
        {
            return new GotifyService(serverInfo, ilogger);
        }
    }
}
