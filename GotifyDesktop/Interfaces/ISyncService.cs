using GotifyDesktop.Models;
using gotifySharp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GotifyDesktop.Service
{
    public interface ISyncService
    {
        event EventHandler<ConnectionStatus> WebSocketConnectionState;
        event EventHandler<MessageModel> OnMessageRecieved;
        
        void Configure(string url, int port, string username, string password, string path, string protocol);
        void InitWebsocket();
        Task<List<MessageModel>> GetMessagesPerAppAsync(int id);
        Task<List<RxApplicationModel>> GetApplicationsAsync();
    }
}