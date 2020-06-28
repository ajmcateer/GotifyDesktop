using GotifyDesktop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using gotifySharp.Models;
using Serilog;
using GotifyDesktop.Exceptions;
using GotifyDesktop.Comparer;
using GotifyDesktop.Infrastructure;

namespace GotifyDesktop.Service
{
    public class SyncService //: AbstractSyncService
    {
        IDatabaseService _databaseService;

        //public SyncService(IDatabaseService databaseService, IGotifyServiceFactory gotifyServiceFactory, ILogger logger) : base(gotifyServiceFactory, logger)
        //{
        //    _databaseService = databaseService;
        //}

        //public override void Update(int appId)
        //{
        //    var db = _databaseService.GetMessagesForApplication(appId);
        //    var messages = _gotifyService.GetMessagesForApplication(appId);
        //}

        //public override async Task FullSyncAsync()
        //{
        //    var applications = await RefreshApplications();

        //    foreach (ApplicationModel app in applications)
        //    {
        //        var messages = await _gotifyService.GetMessagesForApplication(app.id);
        //        _databaseService.InsertMessages(messages);
        //    }
        //}

        //public override async Task IncrementalSyncAsync()
        //{
        //    var applications = await RefreshApplications();

        //    foreach (ApplicationModel app in applications)
        //    {
        //        await GetMessagesForApplication(app.id);
        //    }
        //}

        //public override async Task<List<ApplicationModel>> GetApplicationsAsync()
        //{
        //    if (_lostConnection)
        //    {
        //        await IncrementalSyncAsync();
        //    }
        //    return _databaseService.GetApplications();
        //}

        //public override async Task<List<MessageModel>> GetMessagesPerAppAsync(int appId)
        //{
        //    if (_lostConnection)
        //    {
        //        await IncrementalSyncAsync();
        //    }
        //    return _databaseService.GetMessagesForApplication(appId);
        //}

        /// <summary>
        /// Gets all applications from the server
        /// Removes deleted applications
        /// </summary>
        /// <returns>List of applications</returns>        
        //private async Task<List<ApplicationModel>> RefreshApplications()
        //{
        //    List<ApplicationModel> gotifyApplications = await _gotifyService.GetApplications();
        //    List<ApplicationModel> dbApplications = _databaseService.GetApplications();

        //    RemoveApplicationsFromDb(gotifyApplications, dbApplications);

        //    return GetNewApplications(gotifyApplications, dbApplications);
        //}

        //private List<ApplicationModel> GetNewApplications(List<ApplicationModel> gotifyApplications, List<ApplicationModel> dbApplications)
        //{
        //    foreach (var app in gotifyApplications)
        //    {
        //        _logger.Debug($"Checking for {app.name}");
        //        if (!dbApplications.Any(x => x.id == app.id))
        //        {
        //            _logger.Information($"{app.name} not found adding to list");
        //            _databaseService.InsertApplication(app);
        //        }
        //    }

        //    return gotifyApplications;
        //}

        //private void RemoveApplicationsFromDb(List<ApplicationModel> gotifyApplications, List<ApplicationModel> dbApplications)
        //{
        //    var itemsToRemove = dbApplications.Except(gotifyApplications, new ApplicationComparer()).ToList();
        //    foreach (var item in itemsToRemove)
        //    {
        //        _databaseService.DeleteApplication(item);
        //    }
        //}

        //public override async Task GetMessagesForApplication(int appId)
        //{
        //    try
        //    {
        //        var dbMessages = _databaseService.GetMessagesForApplication(appId);
        //        var highestDbId = dbMessages.OrderByDescending(u => u.id)
        //            .Select(o => o.id)
        //            .FirstOrDefault();

        //        var gotifyMessages = await _gotifyService.GetMessagesForApplication(appId);
        //        var highestGotifyId = gotifyMessages.OrderByDescending(u => u.id)
        //            .Select(o => o.id)
        //            .FirstOrDefault();

        //        if (highestDbId < highestGotifyId)
        //        {
        //            foreach (var message in gotifyMessages)
        //            {
        //                if (message.id > highestDbId)
        //                {
        //                    _databaseService.InsertMessage(message);
        //                }
        //                if (message.id == highestDbId)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        await RemoveMessages(gotifyMessages, dbMessages);
        //    }
        //    catch (SyncFailureException excep)
        //    {
        //        _logger.Error(excep, "Message Sync Failed");
        //    }
        //}

        //private async Task RemoveMessages(List<MessageModel> gotifyMessages, List<MessageModel> dbMessages)
        //{
        //    var itemsToRemove = dbMessages.Except(gotifyMessages, new MessageComparer()).ToList();
        //    foreach (var item in itemsToRemove)
        //    {
        //        _databaseService.DeleteMessage(item);
        //    }
        //}
    }
}
