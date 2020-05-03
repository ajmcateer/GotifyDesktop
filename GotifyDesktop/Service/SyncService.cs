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
    public class SyncService : ISyncService
    {
        private bool _lostConnection = false;
        IDatabaseService _databaseService;
        IGotifyService _gotifyService;
        IGotifyServiceFactory _gotifyServiceFactory;
        ILogger _logger;
        //returns the appid that the message came in on.
        public event EventHandler<int> OnMessageRecieved;
        public event EventHandler<ConnectionStatus> ConnectionState;

        public SyncService(IDatabaseService databaseService, IGotifyServiceFactory gotifyServiceFactory, ILogger logger)
        {
            _databaseService = databaseService;
            _gotifyServiceFactory = gotifyServiceFactory;
            _gotifyService = gotifyServiceFactory.CreateNewGotifyService(logger);
            _logger = logger;
            _gotifyService.OnMessage += GotifyService_OnMessage;
            _gotifyService.ConnectionState += GotifyService_ConnectionState;
        }

        private void GotifyService_ConnectionState(object sender, ConnectionStatus e)
        {
            if(e == ConnectionStatus.Failed && !_lostConnection)
            {
                _lostConnection = true;
            }
            ConnectionState?.Invoke(this, e);
        }

        public void InitWebsocket()
        {
            _gotifyService.InitWebsocket();
        }

        private void GotifyService_OnMessage(object sender, MessageModel e)
        {
            _logger.Information("Message Received");
            _databaseService.InsertMessage(e);
            OnMessageRecieved?.Invoke(this, e.appid);
        }

        public void Update(int appId)
        {
            var db = _databaseService.GetMessagesForApplication(appId);
            var messages = _gotifyService.GetMessagesForApplication(appId);
        }

        public async Task FullSyncAsync()
        {
            var applications = await RefreshApplications();

            foreach (ApplicationModel app in applications)
            {
                var messages = await _gotifyService.GetMessagesForApplication(app.id);
                _databaseService.InsertMessages(messages);
            }
        }

        public async Task IncrementalSyncAsync()
        {
            var applications = await RefreshApplications();

            foreach (ApplicationModel app in applications)
            {
                await GetMessagesForApplication(app.id);
            }
        }

        public async Task<List<ApplicationModel>> GetApplicationsAsync()
        {
            if (_lostConnection)
            {
                await IncrementalSyncAsync();
            }
            return _databaseService.GetApplications();
        }

        public async Task<List<MessageModel>> GetMessagesPerAppAsync(int appId)
        {
            if (_lostConnection)
            {
                await IncrementalSyncAsync();
            }
            return _databaseService.GetMessagesForApplication(appId);
        }

        private async Task<List<ApplicationModel>> RefreshApplications()
        {
            List<ApplicationModel> gotifyApplications = await _gotifyService.GetApplications();
            List<ApplicationModel> dbApplications = _databaseService.GetApplications();

            RemoveApplicationsFromDb(gotifyApplications, dbApplications);

            return GetNewApplications(gotifyApplications, dbApplications);
        }

        private List<ApplicationModel> GetNewApplications(List<ApplicationModel> gotifyApplications, List<ApplicationModel> dbApplications)
        {
            foreach (var app in gotifyApplications)
            {
                _logger.Debug($"Checking for {app.name}");
                if (!dbApplications.Any(x => x.id == app.id))
                {
                    _logger.Information($"{app.name} not found adding to list");
                    _databaseService.InsertApplication(app);
                }
            }

            return gotifyApplications;
        }

        private void RemoveApplicationsFromDb(List<ApplicationModel> gotifyApplications, List<ApplicationModel> dbApplications)
        {
            var itemsToRemove = dbApplications.Except(gotifyApplications, new ApplicationComparer()).ToList();
            foreach (var item in itemsToRemove)
            {
                _databaseService.DeleteApplication(item);
            }
        }

        public async Task GetMessagesForApplication(int appId)
        {
            try
            {
                var dbMessages = _databaseService.GetMessagesForApplication(appId);
                var highestDbId = dbMessages.OrderByDescending(u => u.id)
                    .Select(o => o.id)
                    .FirstOrDefault();

                var gotifyMessages = await _gotifyService.GetMessagesForApplication(appId);
                var highestGotifyId = gotifyMessages.OrderByDescending(u => u.id)
                    .Select(o => o.id)
                    .FirstOrDefault();

                if (highestDbId < highestGotifyId)
                {
                    foreach (var message in gotifyMessages)
                    {
                        if (message.id > highestDbId)
                        {
                            _databaseService.InsertMessage(message);
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
            foreach (var item in itemsToRemove)
            {
                _databaseService.DeleteMessage(item);
            }
        }

        public async Task Configure(string url, int port, string username, string password, string path, string protocol)
        {
            _logger.Information(_gotifyService.GetHashCode().ToString());
            _gotifyService = _gotifyServiceFactory.CreateNewGotifyService(_logger);
            _logger.Information(_gotifyService.GetHashCode().ToString());
            _gotifyService.Configure(url, port, username, password, path, protocol);
        }
    }
}
