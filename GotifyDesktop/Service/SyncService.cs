using GotifyDesktop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using gotifySharp.Models;

namespace GotifyDesktop.Service
{
    public class SyncService
    {
        DatabaseService databaseService;
        GotifyService gotifyService;
        //returns the appid that the message came in on.
        public event EventHandler<int> OnMessageRecieved;
        List<ApplicationModel> applications;

        public SyncService(DatabaseService databaseService, GotifyService gotifyService)
        {
            this.databaseService = databaseService;
            this.gotifyService = gotifyService;
            gotifyService.OnMessage += GotifyService_OnMessage;
            applications = databaseService.GetApplications();
        }

        public async Task Init()
        {
            applications = databaseService.GetApplications();
        }

        private void GotifyService_OnMessage(object sender, MessageModel e)
        {
            databaseService.InsertMessage(e);
            OnMessageRecieved?.Invoke(this, e.appid);
        }

        public void Update(int appId)
        {
            var db = databaseService.GetMessagesForApplication(appId);
            var messages = gotifyService.GetMessagesForApplication(appId);
        }

        public async Task FullSyncAsync()
        {
            await GetApplications();

            foreach (ApplicationModel app in applications)
            {
                var messages = await gotifyService.GetMessagesForApplication(app.id);
                databaseService.InsertMessages(messages);
            }
        }

        public async Task IncrementalSyncAsync()
        {
            await GetApplications();

            foreach (ApplicationModel app in applications)
            {
                await GetMessagesForApplication(app.id);
            }
        }

        private async Task GetApplications()
        {
            List<ApplicationModel> updatedApps = await gotifyService.GetApplications();
            await RemoveApps(updatedApps);

            foreach (var app in updatedApps)
            {
                if (!applications.Contains(app))
                {
                    databaseService.InsertApplication(app);
                }
            }

            applications = updatedApps;
        }

        private async Task RemoveApps(List<ApplicationModel> updatedApps)
        {
            foreach(var app in applications)
            {
                if (!updatedApps.Contains(app))
                {
                    databaseService.DeleteApplication(app);
                }
            }

        }

        public async Task GetMessagesForApplication(int appId)
        {
            var dbMessages = databaseService.GetMessagesForApplication(appId);
            var highestDbId = dbMessages.OrderByDescending(u => u.id)
                .Select(o => o.id)
                .FirstOrDefault();

            var gotifyMessages = await gotifyService.GetMessagesForApplication(appId);
            var highestGotifyId = gotifyMessages.OrderByDescending(u => u.id)
                .Select(o => o.id)
                .FirstOrDefault();

            if (highestDbId < highestGotifyId)
            {
                foreach (var message in gotifyMessages)
                {
                    if (message.id > highestDbId)
                    {
                        databaseService.InsertMessage(message);
                    }
                    if (message.id == highestDbId)
                    {
                        break;
                    }
                }
            }
        }
    }
}
