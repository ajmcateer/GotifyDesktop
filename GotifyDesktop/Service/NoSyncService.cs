using GotifyDesktop.Infrastructure;
using gotifySharp.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.Service
{
    class NoSyncService : AbstractSyncService
    {


        public NoSyncService(IGotifyServiceFactory gotifyServiceFactory, ILogger logger) : base(gotifyServiceFactory, logger)
        {

        }

        public override Task FullSyncAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task<List<ApplicationModel>> GetApplicationsAsync()
        {
           return await _gotifyService.GetApplications();
        }

        public override Task GetMessagesForApplication(int appId)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<MessageModel>> GetMessagesPerAppAsync(int id)
        {
            return await _gotifyService.GetMessagesForApplication(id);
        }

        public override Task IncrementalSyncAsync()
        {
            throw new NotImplementedException();
        }

        public override void Update(int appId)
        {
            throw new NotImplementedException();
        }
    }
}
