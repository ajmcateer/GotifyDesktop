using GotifyDesktop.Infrastructure;
using GotifyDesktop.Models;
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
        public NoSyncService(GotifyService gotifyService, ILogger logger) : base(gotifyService, logger)
        {
            //InitWebsocket();
            _gotifyService.OnMessage += GotifyService_OnMessage;
        }

        public override async Task<List<ExtendedApplicationModel>> GetApplicationsAsync()
        {
            List<ExtendedApplicationModel> applicationModels = new List<ExtendedApplicationModel>();
            var result = await _gotifyService.GetApplications();
            foreach(var obj in result)
            {
                applicationModels.Add(new ExtendedApplicationModel(obj));
            }

            return applicationModels;
        }

        public override async Task<List<MessageModel>> GetMessagesPerAppAsync(int id)
        {
            return await _gotifyService.GetMessagesForApplication(id);
        }
    }
}
