using GotifyDesktop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using gotifySharp.Models;
using Serilog;
using GotifyDesktop.Exceptions;

namespace GotifyDesktop.Service
{
    public class SyncService
    {
        DatabaseService databaseService;
        GotifyService gotifyService;
        ILogger _logger;
        //returns the appid that the message came in on.
        public event EventHandler<int> OnMessageRecieved;
        List<ApplicationModel> applications;

        public SyncService(DatabaseService databaseService, GotifyService gotifyService, ILogger logger)
        {
            this.databaseService = databaseService;
            this.gotifyService = gotifyService;
            this._logger = logger;
            gotifyService.OnMessage += GotifyService_OnMessage;
            applications = databaseService.GetApplications();
        }

        public async Task Init()
        {
            applications = databaseService.GetApplications();
        }

        private void GotifyService_OnMessage(object sender, MessageModel e)
        {
            _logger.Information("Message Received");
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
            try
            {
                List<ApplicationModel> updatedApps = await gotifyService.GetApplications();
                await RemoveApps(updatedApps);

                foreach (var app in updatedApps)
                {
                    _logger.Debug($"Checking for {app.name}");
                    if (!applications.Any(x => x.id == app.id))
                    {
                        _logger.Information($"{app.name} not found adding to list");
                        databaseService.InsertApplication(app);
                    }
                }

                applications = updatedApps;
            }
            catch (SyncFailureException excp)
            {
                _logger.Error(excp, "Application Sync Failed");
            }
        }

        private async Task RemoveApps(List<ApplicationModel> updatedApps)
        {
            foreach(var app in applications)
            {
                _logger.Debug($"Checking for {app.name}");
                if (!updatedApps.Any(x => x.id == app.id))
                {
                    _logger.Information($"{app.name} found deleting from list");
                    databaseService.DeleteApplication(app);
                }
            }
        }

        public async Task GetMessagesForApplication(int appId)
        {
            try
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
            catch (SyncFailureException excep)
            {
                _logger.Error(excep, "Message Sync Failed");
            }
        }
    }
}
