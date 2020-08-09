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

namespace GotifyDesktop.Service
{
    public class GotifyService : IGotifyService
    {
        private GotifySharp gotifySharp;
        ILogger _logger;
        public event EventHandler<MessageModel> OnMessage;
        public event EventHandler<ConnectionStatus> ConnectionState;

        public GotifyService(ILogger logger)
        {
            this._logger = logger;
        }

        public async Task<bool> TestConnectionAsync(string Url, int port, string Username, string Password, string Path, string Protocol)
        {
            //_logger.Information("Starting Connection Test");
            IConfig config = new AppConfig(Username, Password, Url, port, Protocol, Path);
            GotifySharp gotifySharp = new GotifySharp(config);
            //_logger.Debug("GotifySharp Configured");

            try
            {
                var res = await gotifySharp.GetApplications();

                if (res.Success)
                {
                    //_logger.Information("Test Success");
                    return true;
                }
                else
                {
                    //_logger.Information("Test Failed");
                    return false;
                }
            }
            catch (HttpRequestException HttpExcep)
            {
                //_logger.Error(HttpExcep, "Test Failed");
                throw HttpExcep;
            }
        }

        public void Configure(string Url, int port, string Username, string Password, string Path, String Protocol)
        {
            IConfig config = new AppConfig(Username, Password, Url, port, Protocol, Path);
            gotifySharp = new GotifySharp(config);
            gotifySharp.OnMessage += GotifySharp_OnMessage;
            gotifySharp.OnError += GotifySharp_OnError;
            gotifySharp.OnClose += GotifySharp_OnClose;
            gotifySharp.OnOpen += GotifySharp_OnOpen;
        }

        private void GotifySharp_OnOpen(object sender, EventArgs e)
        {
            //TODO Fix Race Condition
            Connection(ConnectionStatus.Successful);
        }

        private void GotifySharp_OnClose(object sender, EventArgs e)
        {
            Connection(ConnectionStatus.Failed);
        }

        private void GotifySharp_OnError(object sender, EventArgs e)
        {
            Connection(ConnectionStatus.Failed);
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
                var response = await gotifySharp.GetApplications();
                foreach (var appResponse in response.ApplicationResponse)
                {
                    applications.Add(appResponse);
                }
                Connection(ConnectionStatus.Successful);
                return applications;
            }
            catch (HttpRequestException httpExc)
            {
                _logger.Error(httpExc, "Unable to Get Applications");
                Connection(ConnectionStatus.Failed);
                throw new SyncFailureException();
            }
        }

        public void InitWebsocket()
        {
            gotifySharp.InitWebsocket();
        }

        public async Task<List<MessageModel>> GetMessagesForApplication(int id)
        {
            try
            {
                List<MessageModel> messages = new List<MessageModel>();
                var messageGetResponse = await gotifySharp.GetMessageForApplicationAsync(id);
                foreach (MessageModel response in messageGetResponse.MessageGetModel.messages)
                {
                    messages.Add(response);
                }
                Connection(ConnectionStatus.Successful);
                return messages;
            }
            catch (HttpRequestException httpExc)
            {
                _logger.Error(httpExc, "Unable to Get Messages");
                Connection(ConnectionStatus.Failed);
                throw new SyncFailureException();
            }
            catch (NullReferenceException nullExcp)
            {
                _logger.Error(nullExcp, "Unable to Get Messages");
                Connection(ConnectionStatus.Failed);
                throw new SyncFailureException();
            }
        }

        private void Connection(ConnectionStatus status)
        {
            ConnectionState?.Invoke(this, status);
        }
    }

    public enum ConnectionStatus
    {
        Failed,
        Successful
    }
}
