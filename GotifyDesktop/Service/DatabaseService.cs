//using GotifyDesktop.Infrastructure;
using GotifyDesktop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using gotifySharp.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Serilog;
using Microsoft.EntityFrameworkCore.Internal;
using GotifyDesktop.Infrastructure;

namespace GotifyDesktop.Service
{
    public class DatabaseService : IDatabaseService
    {
        DatabaseContextFactory databaseContextFactory;
        ILogger _logger;

        public DatabaseService(DatabaseContextFactory databaseContextFactory, ILogger logger)
        {
            this._logger = logger;
            this.databaseContextFactory = databaseContextFactory;
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

        public void ResetDB()
        {
            DeleteDB();
            CreateDB();
        }

        private void DeleteDB()
        {
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                //databaseContext.Database.EnsureDeleted();
                //databaseContext.SaveChanges();
            }
            _logger.Information("DB Deleted");
        }

        private void CreateDB()
        {
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                databaseContext.Database.EnsureCreated();
                databaseContext.SaveChanges();
            }
            _logger.Information("DB Created");
        }

        public void InsertServer(ServerInfo serverInfo)
        {
            _logger.Information($"Inserting {serverInfo.Url}:{serverInfo.Port}/{serverInfo.Path}");

            DeleteServers();

            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                databaseContext.Server.Add(serverInfo);
                databaseContext.SaveChanges();
            }
        }

        public void UpsertServer(ServerInfo serverInfo)
        {
            _logger.Information($"Inserting {serverInfo.Url}:{serverInfo.Port}/{serverInfo.Path}");

            DeleteServers();

            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                databaseContext.Server.Add(serverInfo);
                databaseContext.SaveChanges();
            }
        }

        public void DeleteServers()
        {
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                databaseContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE[Server]");
                databaseContext.SaveChanges();
            }
        }

        public void InsertApplications(List<ApplicationModel> applications)
        {
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                foreach (ApplicationModel app in applications)
                {
                    databaseContext.Applications.Add(app);
                    databaseContext.SaveChanges();
                    var res = databaseContext.Applications.ToList();
                }
            }
        }

        public List<ApplicationModel> GetApplications()
        {
            _logger.Information($"Getting Applications");
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                return databaseContext.Applications.ToList<ApplicationModel>();
            }
        }

        public void InsertApplication(ApplicationModel application)
        {
            _logger.Information($"Inserting {application.name}");
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                databaseContext.Applications.Add(application);
                databaseContext.SaveChanges();
            }
            _logger.Debug("Saved Application");
        }

        public void DeleteApplication(ApplicationModel application)
        {
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                databaseContext.Applications.Remove(application);
                databaseContext.SaveChanges();
            }
        }

        public void DeleteMessage(MessageModel message)
        {
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                databaseContext.Messages.Remove(message);
                databaseContext.SaveChanges();
            }
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
                using (var databaseContext = databaseContextFactory.CreateContext())
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
            }
            catch (Exception e)
            {

            }
        }

        public List<MessageModel> GetMessagesForApplication(int appId)
        {
            _logger.Information($"Getting message for {appId}");
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                var messages = databaseContext.Messages.Where(x => x.appid == appId).ToList<MessageModel>();
                _logger.Debug($"Returning {messages.Count} Messages");
                return messages;
            }
        }

        public List<MessageModel> GetNewMessagesForApplication(int appId, int highestId)
        {
            _logger.Information($"Getting new messages for applications");
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                return databaseContext.Messages.Where(x => x.appid == appId)
                .Where(x => x.id > highestId)
                .ToList<MessageModel>();
            }
        }

        public List<ServerInfo> GetServers()
        {
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                return databaseContext.Server.ToList<ServerInfo>();
            }
        }

        public ServerInfo GetServer()
        {
            using (var databaseContext = databaseContextFactory.CreateContext())
            {
                return databaseContext.Server.FirstOrDefault();
            }
        }
    }
}
