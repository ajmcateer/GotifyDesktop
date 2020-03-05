using GotifyDesktop.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Infrastructure
{
    public class GotifyServiceFactory
    {
        ILogger _ilogger;

        public GotifyServiceFactory(ILogger ilogger)
        {
            _ilogger = ilogger;
        }

        public GotifyService CreateNewGotifyService()
        {
            return new GotifyService(_ilogger);
        }
    }
}
