﻿using Autofac;
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
    public class ApplicationViewModel : ViewModelBase
    {
        private ApplicationModel application;
        IContainer container;
        ObservableCollection<MessageModel> messageModels;
        SyncService syncService;
        DatabaseService databaseService;

        int HighestMessage = 0;

        public ObservableCollection<MessageModel> MessageModels
        {
            get => messageModels;
            set => this.RaiseAndSetIfChanged(ref messageModels, value);
        }

        public ApplicationViewModel(ApplicationModel application, IContainer container)
        {
            MessageModels = new ObservableCollection<MessageModel>();
            this.container = container;
            this.syncService = container.Resolve<SyncService>();
            databaseService = container.Resolve<DatabaseService>();

            this.application = application;

            syncService.OnMessageRecieved += SyncService_OnMessageRecieved;
        }

        private void SyncService_OnMessageRecieved(object sender, int e)
        {
            var res = databaseService.GetMessagesForApplication(e);
            res.Reverse();
            MessageModels = new ObservableCollection<MessageModel>(res);
        }

        public async Task Init()
        {
            await UpdateMessages();
        }

        private async void dispatcherTimer_TickAsync(object sender, EventArgs e)
        {
            await UpdateMessages();
        }

        private async Task UpdateMessages()
        {
            await syncService.GetMessagesForApplication(application.id);
            await GetMessagesFromDBAsync();
        }

        private async Task GetMessagesFromDBAsync()
        {
            MessageModels.Clear();
            var service = container.Resolve<SyncService>();

            try
            {
                var response = databaseService.GetMessagesForApplication(application.id);
                //Display messages in descending order like Gotify Web UI
                response.Reverse();

                foreach (var message in response)
                {
                    MessageModels.Add(message);
                    if(message.id > HighestMessage)
                    {
                        HighestMessage = message.id;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task GetNewMessagesFromDBAsync()
        {
            var service = container.Resolve<SyncService>();

            try
            {
                var response = databaseService.GetNewMessagesForApplication(application.id, HighestMessage);
                //Display messages in descending order like Gotify Web UI
                response.Reverse();

                foreach (var message in response)
                {
                    MessageModels.Insert(0,message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}