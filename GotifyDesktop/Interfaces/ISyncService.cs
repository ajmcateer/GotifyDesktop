using gotifySharp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GotifyDesktop.Service
{
    public interface ISyncService
    {
        event EventHandler<ConnectionStatus> ConnectionState;
        event EventHandler<int> OnMessageRecieved;

        void Configure(string url, int port, string username, string password, string path, string protocol);
        Task FullSyncAsync();
        Task IncrementalSyncAsync();
        Task GetMessagesForApplication(int appId);
        void InitWebsocket();
        void Update(int appId);
        Task<List<MessageModel>> GetMessagesPerAppAsync(int id);
        Task<List<ApplicationModel>> GetApplicationsAsync();
    }
}