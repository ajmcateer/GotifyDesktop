//using GotifyDesktop.Infrastructure;
using GotifyDesktop.Models;
using gotifySharp.Models;
using System.Collections.Generic;

namespace GotifyDesktop.Service
{
    public interface IDatabaseService
    {
        void DeleteApplication(ApplicationModel application);
        void DeleteMessage(MessageModel message);
        List<ApplicationModel> GetApplications();
        List<MessageModel> GetMessagesForApplication(int appId);
        List<MessageModel> GetNewMessagesForApplication(int appId, int highestId);
        ServerInfo GetServer();
        List<ServerInfo> GetServers();
        void InsertApplication(ApplicationModel application);
        void InsertApplications(List<ApplicationModel> applications);
        void InsertMessage(MessageModel message);
        void InsertMessages(List<MessageModel> messages);
        void InsertServer(ServerInfo serverInfo);
        void ResetDB();
    }
}