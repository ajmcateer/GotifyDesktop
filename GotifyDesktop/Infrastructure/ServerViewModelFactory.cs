using Autofac;
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
        GotifyServiceFactory _gotifyServiceFactory;

        public ServerViewModelFactory(GotifyServiceFactory gotifyServiceFactory)
        {
            _gotifyServiceFactory = gotifyServiceFactory;
        }

        public ServerViewModelFactory()
        {

        }

        public ServerViewModel GetNewServerViewModel(GotifyService gotifyService)
        {
            //return new ServerViewModel(_gotifyServiceFactory, _settingsViewModel.InitRouting(screen), screen);
            return new ServerViewModel(gotifyService);
        }
    }
}
