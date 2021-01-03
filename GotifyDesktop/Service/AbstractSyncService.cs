using GotifyDesktop.Infrastructure;
using GotifyDesktop.Models;
using gotifySharp.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static gotifySharp.Enums.ConnectionInfo;

namespace GotifyDesktop.Service
{
    public abstract class AbstractSyncService : ISyncService
    {
        public event EventHandler<ConnectionStatus> WebSocketConnectionState;
        public event EventHandler<MessageModel> OnMessageRecieved;
        private ConnectionStatus _connectionStatus = ConnectionStatus.Failed;

        internal bool _lostConnection = false;
        internal IGotifyService _gotifyService;
        internal ILogger _logger;

        public AbstractSyncService(GotifyService gotifyService, ILogger logger)
        {
            _gotifyService = gotifyService;
            _logger = logger;
        }

        public virtual void InitWebsocket()
        {
            _gotifyService.InitWebsocket();
        }

        internal virtual void GotifyService_OnMessage(object sender, MessageModel e)
        {
            OnMessageRecieved?.Invoke(this, e);
        }

        public virtual void Configure(string url, int port, string username, string password, string path, string protocol)
        {
            _logger.Information(_gotifyService.GetHashCode().ToString());
            _gotifyService.Configure(url, port, username, password, path, protocol);
            InitWebsocket();
            _gotifyService.OnMessage += GotifyService_OnMessage;
            _gotifyService.OnDisconnect += _gotifyService_OnDisconnect;
            _gotifyService.OnReconnect += _gotifyService_OnReconnect;
        }

        private void _gotifyService_OnReconnect(object sender, WebsocketReconnectStatus e)
        {
            WebSocketConnectionState?.Invoke(this, ConnectionStatus.Successful);
        }

        private void _gotifyService_OnDisconnect(object sender, WebsocketDisconnectStatus e)
        {
            if(e != WebsocketDisconnectStatus.NoMessageReceived)
            {
                WebSocketConnectionState?.Invoke(this, ConnectionStatus.Failed);
            }
        }

        public abstract Task<List<ExtendedApplicationModel>> GetApplicationsAsync();

        public abstract Task<List<MessageModel>> GetMessagesPerAppAsync(int id);
    }
}
