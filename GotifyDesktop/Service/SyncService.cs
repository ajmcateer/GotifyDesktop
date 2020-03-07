using GotifyDesktop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using gotifySharp.Models;
using Serilog;
using GotifyDesktop.Exceptions;
using GotifyDesktop.Comparer;
using GotifyDesktop.Infrastructure;

namespace GotifyDesktop.Service
{
    public class SyncService
    {
        DatabaseService databaseService;
        GotifyService gotifyService;
        GotifyServiceFactory gotifyServiceFactory;
        ILogger _logger;
        //returns the appid that the message came in on.
        public event EventHandler<int> OnMessageRecieved;
        public event EventHandler<ConnectionStatus> ConnectionState;

        public SyncService(DatabaseService databaseService, GotifyServiceFactory gotifyServiceFactory, ILogger logger)
        {
            this.databaseService = databaseService;
            this.gotifyService = gotifyServiceFactory.CreateNewGotifyService();
            this.gotifyServiceFactory = gotifyServiceFactory;
            this._logger = logger;
            gotifyService.OnMessage += GotifyService_OnMessage;
            gotifyService.ConnectionState += GotifyService_ConnectionState;
        }

        private void GotifyService_ConnectionState(object sender, ConnectionStatus e)
        {
            ConnectionState?.Invoke(this, e);
        }

        public void InitWebsocket()
        {
            gotifyService.InitWebsocket();
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
            var applications = await GetApplications();

            foreach (ApplicationModel app in applications)
            {
                var messages = await gotifyService.GetMessagesForApplication(app.id);
                databaseService.InsertMessages(messages);
            }
        }

        //public async Task IncrementalSyncAsync()
        //{
        //    var applications = await GetApplications();

        //    foreach (ApplicationModel app in applications)
        //    {
        //        await GetMessagesForApplication(app.id);
        //    }
        //}

        private async Task<List<ApplicationModel>> GetApplications()
        {
            List<ApplicationModel> gotifyApplications = await gotifyService.GetApplications();
            List<ApplicationModel> dbApplications = databaseService.GetApplications();

            await RemoveApps(gotifyApplications, dbApplications);

            foreach (var app in gotifyApplications)
            {
                _logger.Debug($"Checking for {app.name}");
                if (!dbApplications.Any(x => x.id == app.id))
                {
                    _logger.Information($"{app.name} not found adding to list");
                    databaseService.InsertApplication(app);
                }
            }

            return gotifyApplications;
        }

        private async Task RemoveApps(List<ApplicationModel> gotifyApplications, List<ApplicationModel> dbApplications)
        {
            var itemsToRemove = dbApplications.Except(gotifyApplications, new ApplicationComparer()).ToList();
            foreach (var item in itemsToRemove)
            {
                databaseService.DeleteApplication(item);
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
                await RemoveMessages(gotifyMessages, dbMessages);
            }
            catch (SyncFailureException excep)
            {
                _logger.Error(excep, "Message Sync Failed");
            }
        }

        private async Task RemoveMessages(List<MessageModel> gotifyMessages, List<MessageModel> dbMessages)
        {
            var itemsToRemove = dbMessages.Except(gotifyMessages, new MessageComparer()).ToList();
            foreach(var item in itemsToRemove)
            {
                databaseService.DeleteMessage(item);
            }
        }

        public async Task Configure(string url, int port, string username, string password, string path, string protocol)
        {
            _logger.Information(gotifyService.GetHashCode().ToString());
            gotifyService = gotifyServiceFactory.CreateNewGotifyService();
            _logger.Information(gotifyService.GetHashCode().ToString());
            gotifyService.Configure(url, port, username, password, path, protocol);
        }
    }
}
