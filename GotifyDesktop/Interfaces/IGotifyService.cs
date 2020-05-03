using gotifySharp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GotifyDesktop.Service
{
    public interface IGotifyService
    {
        event EventHandler<ConnectionStatus> ConnectionState;
        event EventHandler<MessageModel> OnMessage;

        void Configure(string Url, int port, string Username, string Password, string Path, string Protocol);
        Task<List<ApplicationModel>> GetApplications();
        Task<List<MessageModel>> GetMessagesForApplication(int id);
        void InitWebsocket();
        Task<bool> TestConnectionAsync(string Url, int port, string Username, string Password, string Path, string Protocol);
    }
}