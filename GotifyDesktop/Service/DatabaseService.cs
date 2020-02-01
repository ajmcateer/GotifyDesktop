using GotifyDesktop.Infrastructure;
using GotifyDesktop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using gotifySharp.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace GotifyDesktop.Service
{
    public class DatabaseService
    {
        DatabaseContext databaseContext;

        public DatabaseService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
            CheckDB();
        }

        private void CheckDB()
        {
            if (!File.Exists("gotifyDesktop.db"))
            {
                CreateDB();
            }
        }

        private void CreateDB()
        {
            databaseContext.Database.EnsureCreated();
        }

        public void InsertServer(ServerInfo serverInfo)
        {
            databaseContext.Server.Add(serverInfo);
            databaseContext.SaveChanges();
        }

        public void InsertApplications(List<ApplicationModel> applications)
        {
            foreach(ApplicationModel app in applications)
            {
                InsertApplication(app);
            }
        }

        public List<ApplicationModel> GetApplications()
        {
            return databaseContext.Applications.ToList<ApplicationModel>();
        }

        public void InsertApplication(ApplicationModel application)
        {
            databaseContext.Applications.Add(application);
            databaseContext.SaveChanges();
        }

        public void DeleteApplication(ApplicationModel application)
        {
            databaseContext.Applications.Remove(application);
            databaseContext.SaveChanges();
        }

        public void InsertMessages(List<MessageModel> messages)
        {
            foreach (MessageModel message in messages)
            {
                InsertMessage(message);
            }
        }

        public void InsertMessage(MessageModel message)
        {
            try
            {
                var isThere = databaseContext.Messages.Where(x => x.id == message.id).Any();
                if (!isThere)
                {
                    databaseContext.Messages.Add(message);
                    databaseContext.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }
        }

        public List<MessageModel> GetMessagesForApplication(int appId)
        {
            return databaseContext.Messages.Where(x => x.appid == appId).ToList<MessageModel>();
        }

        public List<MessageModel> GetNewMessagesForApplication(int appId, int highestId)
        {
            return databaseContext.Messages.Where(x => x.appid == appId)
                .Where(x => x.id > highestId)
                .ToList<MessageModel>();
        }
    }
}
