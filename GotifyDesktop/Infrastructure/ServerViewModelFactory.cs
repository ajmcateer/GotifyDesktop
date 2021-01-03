using GotifyDesktop.Models;
using GotifyDesktop.Service;
using GotifyDesktop.ViewModels;
using ReactiveUI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Infrastructure
{
    public class ServerViewModelFactory
    {
        ILogger _ilogger;
        GotifyServiceFactory _gotifyServiceFactory;

        public ServerViewModelFactory(GotifyServiceFactory gotifyServiceFactory, ILogger ilogger)
        {
            _gotifyServiceFactory = gotifyServiceFactory;
            _ilogger = ilogger;
        }

        public ServerViewModel GetNewServerViewModel(ServerInfo serverInfo)
        {
            var test = _gotifyServiceFactory.CreateNewGotifyService(serverInfo, _ilogger) as GotifyService;
            return new ServerViewModel(new NoSyncService(test, _ilogger));
        }
    }
}
