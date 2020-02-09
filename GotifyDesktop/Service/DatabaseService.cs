using GotifyDesktop.Infrastructure;
using GotifyDesktop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using gotifySharp.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Serilog;

namespace GotifyDesktop.Service
{
    public class DatabaseService
    {
        DatabaseContext databaseContext;
        ILogger _logger;

        public DatabaseService(DatabaseContext databaseContext, ILogger logger)
        {
            this._logger = logger;
            this.databaseContext = databaseContext;
            CheckDB();
        }

        private void CheckDB()
        {
            if (!File.Exists("gotifyDesktop.db"))
            {
                _logger.Information("DB does not exist Creating DB");
                CreateDB();
            }
        }

        private void CreateDB()
        {
            databaseContext.Database.EnsureCreated();
            _logger.Debug("DB Created");
        }

        public void InsertServer(ServerInfo serverInfo)
        {
            _logger.Information($"Inserting {serverInfo.Url}:{serverInfo.Port}/{serverInfo.Path}");
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
            _logger.Information($"Getting Applications");
            return databaseContext.Applications.ToList<ApplicationModel>();
        }

        public void InsertApplication(ApplicationModel application)
        {
            _logger.Information($"Inserting {application.name}");
            databaseContext.Applications.Add(application);
            databaseContext.SaveChanges();
            _logger.Debug("Saved Application");
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
                    _logger.Information($"Inserting {message.id}");
                    databaseContext.Messages.Add(message);
                    databaseContext.SaveChanges();
                    _logger.Debug("Message Saved");
                }
            }
            catch (Exception e)
            {

            }
        }

        public List<MessageModel> GetMessagesForApplication(int appId)
        {
            _logger.Information($"Getting message for {appId}");
            var messages = databaseContext.Messages.Where(x => x.appid == appId).ToList<MessageModel>();
            _logger.Debug($"Returning {messages.Count} Messages");
            return messages;
        }

        public List<MessageModel> GetNewMessagesForApplication(int appId, int highestId)
        {
            _logger.Information($"Getting new messages for applications");
            return databaseContext.Messages.Where(x => x.appid == appId)
                .Where(x => x.id > highestId)
                .ToList<MessageModel>();
        }
    }
}
