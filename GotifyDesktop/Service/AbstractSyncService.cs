using GotifyDesktop.Infrastructure;
using GotifyDesktop.Models;
using gotifySharp.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.Service
{
    public abstract class AbstractSyncService : ISyncService
    {
        public event EventHandler<ConnectionStatus> ConnectionState;
        public event EventHandler<int> OnMessageRecieved;

        internal bool _lostConnection = false;
        internal IGotifyService _gotifyService;
        internal IGotifyServiceFactory _gotifyServiceFactory;
        internal ILogger _logger;

        public AbstractSyncService(IGotifyServiceFactory gotifyServiceFactory, ILogger logger)
        {
            _gotifyServiceFactory = gotifyServiceFactory;
            _logger = logger;
        }

        internal virtual void GotifyService_ConnectionState(object sender, ConnectionStatus e)
        {
            if (e == ConnectionStatus.Failed && !_lostConnection)
            {
                _lostConnection = true;
            }
            ConnectionState?.Invoke(this, e);
        }

        public virtual void InitWebsocket()
        {
            _gotifyService.InitWebsocket();
        }

        internal virtual void GotifyService_OnMessage(object sender, MessageModel e)
        {
            _logger.Information("Message Received");
            OnMessageRecieved?.Invoke(this, e.appid);
        }

        public virtual void Configure(string url, int port, string username, string password, string path, string protocol)
        {
            //_logger.Information(_gotifyService.GetHashCode().ToString());
            _gotifyService = _gotifyServiceFactory.CreateNewGotifyService(_logger);
            _logger.Information(_gotifyService.GetHashCode().ToString());
            _gotifyService.Configure(url, port, username, password, path, protocol);
            InitWebsocket();
            _gotifyService.OnMessage += GotifyService_OnMessage;
            _gotifyService.ConnectionState += GotifyService_ConnectionState;
        }

        public abstract Task<List<ExtendedApplicationModel>> GetApplicationsAsync();

        public abstract Task<List<MessageModel>> GetMessagesPerAppAsync(int id);
    }
}
