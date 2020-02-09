using Autofac;
using GotifyDesktop.Service;
using gotifySharp.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class MainControlv2ViewModel : ViewModelBase
    {
        IContainer container;
        ObservableCollection<ApplicationModel> applications;
        ApplicationModel selectedApplication;
        ObservableCollection<MessageModel> messageModels;

        DatabaseService databaseService;
        SyncService syncService;

        public ObservableCollection<MessageModel> MessageModels
        {
            get => messageModels;
            set => this.RaiseAndSetIfChanged(ref messageModels, value);
        }

        public ApplicationModel SelectedItem
        {
            get => selectedApplication;
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedApplication, value);
                UpdateMessageDisplay();
            }
        }

        public ObservableCollection<ApplicationModel> Applications
        {
            get => applications;
            private set => this.RaiseAndSetIfChanged(ref applications, value);
        }

        public MainControlv2ViewModel(IContainer container)
        {
            this.container = container;
            messageModels = new ObservableCollection<MessageModel>();
            applications = new ObservableCollection<ApplicationModel>();

            databaseService = container.Resolve<DatabaseService>();
            syncService = container.Resolve<SyncService>();
            syncService.OnMessageRecieved += SyncService_OnMessageRecieved;
        }

        private void SyncService_OnMessageRecieved(object sender, int e)
        {
            UpdateMessageDisplay();
        }

        private void UpdateMessageDisplay()
        {
            var res = databaseService.GetMessagesForApplication(SelectedItem.id);
            res.Reverse();
            MessageModels = new ObservableCollection<MessageModel>(res);
        }

        public async Task InitAsync()
        {
            try
            {
                List<ApplicationModel> results = databaseService.GetApplications();
                Applications = new ObservableCollection<ApplicationModel>(results);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}