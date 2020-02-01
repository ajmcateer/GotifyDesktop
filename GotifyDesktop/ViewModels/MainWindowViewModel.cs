using Autofac;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Service;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;
        AddServerViewModel addServerViewModel;
        MainControlViewModel MainControlViewModel;
        MainControlv2ViewModel MainControlv2;
        IContainer container;

        DatabaseContext dbcontext;
        SettingService settings;
        GotifyService gotifyService;
        SyncService syncService;

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public MainWindowViewModel(IContainer container)
        {
            this.container = container;

            settings = container.Resolve<SettingService>();
            dbcontext = container.Resolve<DatabaseContext>();
            gotifyService = container.Resolve<GotifyService>();
            syncService = container.Resolve<SyncService>();

            addServerViewModel = new AddServerViewModel(container);
            addServerViewModel.saveServerEvent += AddServerViewModel_saveServerEventAsync;
            MainControlv2 = new MainControlv2ViewModel(container);
        }

        private async void AddServerViewModel_saveServerEventAsync()
        {
            Initialize(this, null);
        }

        public async void Initialize(object sender, object parameter)
        {
            var server = dbcontext.Server.Find(1);

            if (server == null)
            {
                Content = addServerViewModel;
            }
            else
            {
                gotifyService.Configure(server.Url, server.Port, server.Username, server.Password, server.Path, server.Protocol);
            }

            await syncService.FullSyncAsync();
            await MainControlv2.initAsync();
            gotifyService.InitWebsocket();
            Content = MainControlv2;
        }
    }
}
