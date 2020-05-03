using GotifyDesktop.Infrastructure;
using GotifyDesktop.Service;
using gotifySharp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GotifyDesktopUnitTests
{
    [TestClass]
    public class SyncServiceTests
    {
        [TestMethod]
        public async Task GetApplicationsSameTestAsync()
        {
            var Apps = Helper.GenerateApplicationList(3);

            var mockLogger = new Mock<ILogger>();
            var mockDatabaseService = new Mock<IDatabaseService>();
            mockDatabaseService.Setup(x => x.GetApplications()).Returns(Apps);

            var mockGotifyService = new Mock<IGotifyService>();
            //mockGotifyService.Setup<Task<IList<ApplicationModel>>>(x => x.GetApplications()).ReturnsAsync(Task.FromResult<IList<ApplicationModel>>(Apps));
            mockGotifyService.Setup(x => x.GetApplications()).ReturnsAsync(Apps);
            mockGotifyService.SetupAdd(m => m.OnMessage += It.IsAny<EventHandler<MessageModel>>());
            mockGotifyService.SetupAdd(m => m.ConnectionState += It.IsAny<EventHandler<ConnectionStatus>>());

            var mockGotifyServiceFactory = new Mock<IGotifyServiceFactory>();
            mockGotifyServiceFactory.Setup(x => x.CreateNewGotifyService(mockLogger.Object)).Returns(mockGotifyService.Object);

            SyncService syncService = new SyncService(mockDatabaseService.Object, mockGotifyServiceFactory.Object, mockLogger.Object);

            await syncService.IncrementalSyncAsync();

            //Action act = () => { databaseService.InsertApplications(dbApps); };

            //act.Should().NotThrow();
        }
    }
}
