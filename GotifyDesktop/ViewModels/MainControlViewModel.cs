using Autofac;
using GotifyDesktop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GotifyDesktop.Service;
using gotifySharp.Responses;
using ReactiveUI;
using gotifySharp.Models;
using GotifyDesktop.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace GotifyDesktop.ViewModels
{
    public class MainControlViewModel : ViewModelBase
    {
        IContainer container;
        ObservableCollection<ApplicationModel> applications;
        ApplicationModel selectedApplicationViewModel;
        int selectedIndex; 
        
        ObservableCollection<TabItem> tabItems;
        ObservableCollection<MessageModel> messageModels;

        DatabaseService databaseService;
        SyncService syncService;

        public ObservableCollection<MessageModel> MessageModels
        {
            get => messageModels;
            set => this.RaiseAndSetIfChanged(ref messageModels, value);
        }

        public ObservableCollection<TabItem> TabItems
        {
            get => tabItems;
            set => this.RaiseAndSetIfChanged(ref tabItems, value);
        }

        public int SelectedIndex
        {
            get => selectedIndex;
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedIndex, value);
            }
        }

        public ApplicationModel SelectedApplicationViewModel
        {
            get => selectedApplicationViewModel;
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedApplicationViewModel, value);
            }
        }

        public ObservableCollection<ApplicationModel> Applications
        {
            get => applications;
            private set => this.RaiseAndSetIfChanged(ref applications, value);
        }

        public MainControlViewModel(IContainer container)
        {
            this.container = container;

            tabItems = new ObservableCollection<TabItem>();
            messageModels = new ObservableCollection<MessageModel>();
            applications = new ObservableCollection<ApplicationModel>();

            databaseService = container.Resolve<DatabaseService>();
            syncService = container.Resolve<SyncService>();
        }

        public async Task initAsync()
        {
            try
            {
                List<ApplicationModel> results = databaseService.GetApplications();
                Applications = new ObservableCollection<ApplicationModel>(results);
                foreach(var app in Applications)
                {
                    var appView = new ApplicationViewModel(app, container);
                    await appView.Init();
                    TabItems.Add(new TabItem{Header = app.name, Content = appView});
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
