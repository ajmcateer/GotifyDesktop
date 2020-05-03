using GotifyDesktop.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Infrastructure
{
    public class GotifyServiceFactory : IGotifyServiceFactory
    {
        public IGotifyService CreateNewGotifyService(ILogger ilogger)
        {
            return new GotifyService(ilogger);
        }
    }
}
