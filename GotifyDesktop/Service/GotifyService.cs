using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GotifyDesktop.Exceptions;
using GotifyDesktop.Models;
using gotifySharp;
using gotifySharp.Interfaces;
using gotifySharp.Models;
using gotifySharp.Responses;
using Serilog;
using static gotifySharp.Enums.ConnectionInfo;
using gotifySharp.Api;

namespace GotifyDesktop.Service
{
    public class GotifyService : IGotifyService
    {
        private GotifySharp gotifySharp;
        ILogger _logger;
        ServerInfo _serverInfo;

        public event EventHandler<MessageModel> OnMessage;
        public event EventHandler<WebsocketDisconnectStatus> OnDisconnect;
        public event EventHandler<WebsocketReconnectStatus> OnReconnect;
        
        public GotifyService(ServerInfo serverInfo)
        {
            _serverInfo = serverInfo;
            IConfig config = new AppConfig(serverInfo.Username, serverInfo.Password, serverInfo.Url, serverInfo.Port, serverInfo.Protocol, serverInfo.Path);
            //gotifySharp = new GotifySharp(config);
            //gotifySharp.OnMessage += GotifySharp_OnMessage;
            //gotifySharp.OnDisconnect += GotifySharp_OnDisconnect;
            //gotifySharp.OnReconnect += GotifySharp_OnReconnect;
        }

        //public static async Task<bool> TestConnectionAsync(string Url, int port, string Username, string Password, string Path, string Protocol)
        //{
        //    IConfig config = new AppConfig(Username, Password, Url, port, Protocol, Path);
        //    GotifySharp gotifySharp = new GotifySharp(config);

        //    try
        //    {
        //        //var res = await gotifySharp.GetApplications();

        //        //if (res.Success)
        //        //{
        //        //    return true;
        //        //}
        //        //else
        //        //{
        //        //    return false;
        //        //}
        //    }
        //    catch (HttpRequestException HttpExcep)
        //    {
        //        throw HttpExcep;
        //    }
        //}

        public void Configure(string Url, int port, string Username, string Password, string Path, String Protocol)
        {
            //IConfig config = new AppConfig(Username, Password, Url, port, Protocol, Path);
            //gotifySharp = new GotifySharp(config);
            //gotifySharp.OnMessage += GotifySharp_OnMessage;
            //gotifySharp.OnDisconnect += GotifySharp_OnDisconnect;
            //gotifySharp.OnReconnect += GotifySharp_OnReconnect;
        }

        private void GotifySharp_OnReconnect(object sender, WebsocketReconnectStatus e)
        {
            OnReconnect?.Invoke(this, e);
        }

        private void GotifySharp_OnDisconnect(object sender, WebsocketDisconnectStatus e)
        {
            OnDisconnect?.Invoke(this, e);
        }

        private void GotifySharp_OnMessage(object sender, MessageModel e)
        {
            OnMessage?.Invoke(this, e);
        }

        public async Task<List<ApplicationModel>> GetApplications()
        {
            try
            {
                List<ApplicationModel> applications = new List<ApplicationModel>();
                //var response = await gotifySharp.GetApplications();
                //foreach (var appResponse in response.ApplicationResponse)
                //{
                //    applications.Add(appResponse);
                //}
                return applications;
            }
            catch (HttpRequestException httpExc)
            {
                _logger.Error(httpExc, "Unable to Get Applications");
                throw new SyncFailureException();
            }
        }

        public void InitWebsocket()
        {
            //gotifySharp.InitWebsocket();
        }

        public async Task<List<MessageModel>> GetMessagesForApplication(int id)
        {
            try
            {
                List<MessageModel> messages = new List<MessageModel>();
                //var messageGetResponse = await gotifySharp.GetMessageForApplicationAsync(id);
                //foreach (MessageModel response in messageGetResponse.MessageGetModel.messages)
                //{
                //    messages.Add(response);
                //}
                return messages;
            }
            catch (HttpRequestException httpExc)
            {
                _logger.Error(httpExc, "Unable to Get Messages");
                throw new SyncFailureException();
            }
            catch (NullReferenceException nullExcp)
            {
                _logger.Error(nullExcp, "Unable to Get Messages");
                throw new SyncFailureException();
            }
        }
    }

    public enum ConnectionStatus
    {
        Failed,
        Successful
    }
}
