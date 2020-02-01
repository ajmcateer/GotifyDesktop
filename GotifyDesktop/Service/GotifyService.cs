using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GotifyDesktop.Models;
using gotifySharp;
using gotifySharp.Interfaces;
using gotifySharp.Models;
using gotifySharp.Responses;

namespace GotifyDesktop.Service
{
    public class GotifyService
    {
        private GotifySharp gotifySharp;
        public event EventHandler<MessageModel> OnMessage;

        public async Task<bool> TestConnectionAsync(string Url, int port, string Username, string Password, string Path, String Protocol)
        {
            IConfig config = new AppConfig(Username, Password, Url, port, Protocol, Path);
            GotifySharp _gotifySharp = new GotifySharp(config);
            var res = await _gotifySharp.GetApplications();

            if (res.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Configure(string Url, int port, string Username, string Password, string Path, String Protocol)
        {
            IConfig config = new AppConfig(Username, Password, Url, port, Protocol, Path);
            gotifySharp = new GotifySharp(config);
            gotifySharp.OnMessage += GotifySharp_OnMessage;
        }

        private void GotifySharp_OnMessage(object sender, MessageModel e)
        {
            OnMessage?.Invoke(this, e);
        }

        public async Task<List<ApplicationModel>> GetApplications()
        {
            List<ApplicationModel> applications = new List<ApplicationModel>();
            var response = await gotifySharp.GetApplications();
            foreach(var appResponse in response.ApplicationResponse)
            {
                ApplicationModel app = new ApplicationModel();
                app.description = appResponse.description;
                app.id = appResponse.id;
                app.image = appResponse.image;
                app.name = appResponse.name;
                app.token = appResponse.token;
                app._internal = appResponse._internal;
                applications.Add(app);
            }
            return applications;
        }

        public void InitWebsocket()
        {
            gotifySharp.InitWebsocket();
        }

        public async Task<List<MessageModel>> GetMessagesForApplication(int id)
        {
            List<MessageModel> messages = new List<MessageModel>();
            var messageGetResponse = await gotifySharp.GetMessageForApplicationAsync(id);
            foreach(MessageModel response in messageGetResponse.MessageGetModel.messages)
            {
                MessageModel mess = new MessageModel();
                mess.appid = response.appid;
                mess.date = response.date;
                mess.id = response.id;
                mess.message = response.message;
                mess.priority = response.priority;
                mess.title = response.title;
                messages.Add(mess);
            }
            return messages;
        }
    }
}
