using GotifyDesktop.Infrastructure;
using GotifyDesktop.Service;
using gotifySharp.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotifyDesktopUnitTests
{
    internal static class Helper
    {
        private static int id = 1;

        public static ApplicationModel GenerateApplication()
        {
            ApplicationModel applicationModel = new ApplicationModel();
            applicationModel.description = RandomString(20);
            applicationModel.id = GetId();
            applicationModel.name = RandomString(10);
            applicationModel.token = RandomString(8);

            return applicationModel;
        }

        public static List<ApplicationModel> GenerateApplicationList(int numOfApps)
        {
            List<ApplicationModel> apps = new List<ApplicationModel>();

            for (int x = 0; x < numOfApps; x++)
            {
                apps.Add(GenerateApplication());
            }

            return apps;
        }

        public static MessageModel GenerateMessage(int appId = 1)
        {
            MessageModel messageModel = new MessageModel();
            messageModel.appid = appId;
            messageModel.date = DateTime.Now;
            messageModel.id = GetId();
            messageModel.message = RandomString(50);
            messageModel.priority = 2;
            messageModel.title = RandomString(10);

            return messageModel;
        }

        public static List<MessageModel> GenerateMessageList(int numOfMessages, int appId = 1)
        {
            List<MessageModel> messages = new List<MessageModel>();

            for (int x = 0; x < numOfMessages; x++)
            {
                messages.Add(GenerateMessage(appId));
            }

            return messages;
        }

        public static Tuple<List<ApplicationModel>, List<MessageModel>> GenerateAppsWithMessages()
        {
            List<ApplicationModel> applicationModels = new List<ApplicationModel>();
            List<MessageModel> messageModels = new List<MessageModel>();

            applicationModels = GenerateApplicationList(3);
            
            foreach(var app in applicationModels)
            {
                messageModels.AddRange(GenerateMessageList(3, app.id));
            }

            return new Tuple<List<ApplicationModel>, List<MessageModel>>(applicationModels, messageModels);
        }

        public static Tuple<List<ApplicationModel>, List<ApplicationModel>> GenerateSimiliarApplicationList(int numOfApps) => throw new NotImplementedException();

        private static int GetId()
        {
            var numToReturn = id;
            id++;
            return numToReturn;
        }

        //https://stackoverflow.com/a/9995910/3864956
        private static string RandomString(int length)
        {
            Random rand = new Random();
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[rand.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }
    }
}
