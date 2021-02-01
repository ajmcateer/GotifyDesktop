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
        SettingsViewModel _settingsViewModel;

        public ServerViewModelFactory(GotifyServiceFactory gotifyServiceFactory, SettingsViewModel settingsViewModel)
        {
            _gotifyServiceFactory = gotifyServiceFactory;
            _settingsViewModel = settingsViewModel;
        }

        public ServerViewModel GetNewServerViewModel(IScreen screen)
        {
            return new ServerViewModel(_gotifyServiceFactory, _settingsViewModel.InitRouting(screen), screen);
        }
    }
}
