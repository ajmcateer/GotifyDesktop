using gotifySharp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static gotifySharp.Enums.ConnectionInfo;

namespace GotifyDesktop.Service
{
    public interface IGotifyService
    {
        event EventHandler<WebsocketDisconnectStatus> OnDisconnect;
        event EventHandler<WebsocketReconnectStatus> OnReconnect;
        event EventHandler<MessageModel> OnMessage;

        void Configure(string Url, int port, string Username, string Password, string Path, string Protocol);
        Task<List<ApplicationModel>> GetApplications();
        Task<List<MessageModel>> GetMessagesForApplication(int id);
        void InitWebsocket();
        Task<bool> TestConnectionAsync(string Url, int port, string Username, string Password, string Path, string Protocol);
    }
}