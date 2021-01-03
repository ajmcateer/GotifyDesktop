using Autofac;
using Avalonia.Threading;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Models;
using GotifyDesktop.Service;
using gotifySharp.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class ApplicationViewModela : ViewModelBase
    {
        private ApplicationModel application;
        ObservableCollection<MessageModel> messageModels;
        ISyncService syncService;

        public ObservableCollection<MessageModel> MessageModels
        {
            get => messageModels;
            set => this.RaiseAndSetIfChanged(ref messageModels, value);
        }

        public ApplicationViewModela(ApplicationModel application, ISyncService syncService)
        {
            MessageModels = new ObservableCollection<MessageModel>();
            this.syncService = syncService;

            this.application = application;

            syncService.OnMessageRecieved += SyncService_OnMessageRecieved;
        }

        private void SyncService_OnMessageRecieved(object sender, MessageModel e)
        {
            //var res = databaseService.GetMessagesForApplication(e);
            //res.Reverse();
            //MessageModels = new ObservableCollection<MessageModel>(res);
        }
    }
}