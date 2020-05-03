using GotifyDesktop.Infrastructure;
using GotifyDesktop.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.EntityFrameworkCore;
using Serilog;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GotifyDesktop.Comparer;

namespace GotifyDesktopUnitTests
{
    [TestClass]
    public class DatabaseServiceUnitTests
    {
        [TestMethod]
        public void InsertApplicationListTest()
        {
            var dbApps = Helper.GenerateApplicationList(3);

            var databaseService = Helper.BuildDbService();

            Action act = () => { databaseService.InsertApplications(dbApps); };

            act.Should().NotThrow();
        }

        [TestMethod]
        public void InsertApplicationTest()
        {
            var dbApp = Helper.GenerateApplication();

            var databaseService = Helper.BuildDbService();

            Action act = () => { databaseService.InsertApplication(dbApp); };

            act.Should().NotThrow();
        }

        [TestMethod]
        public void InsertMessageTest()
        {
            var dbMess = Helper.GenerateMessage();

            var databaseService = Helper.BuildDbService();

            Action act = () => { databaseService.InsertMessage(dbMess); };

            act.Should().NotThrow();
        }

        [TestMethod]
        public void InsertMessageListTest()
        {
            var dbMess = Helper.GenerateMessageList(3);

            var databaseService = Helper.BuildDbService();

            Action act = () => { databaseService.InsertMessages(dbMess); };

            act.Should().NotThrow();
        }

        [TestMethod]
        public void GetApplicationTest()
        {
            var dbApps = Helper.GenerateApplicationList(3);

            var databaseService = Helper.BuildDbService();

            databaseService.InsertApplications(dbApps);

            var retrivedList = databaseService.GetApplications();

            var res = dbApps
                .Except(retrivedList, new ApplicationComparer())
                .ToList()
                .Should()
                .BeEmpty();
        }

        [TestMethod]
        public void GetMessagePerAppTest()
        {
            var fakeDB = Helper.GenerateAppsWithMessages();
            var dbApps = fakeDB.Item1;
            var dbMess = fakeDB.Item2;

            var databaseService = Helper.BuildDbService();

            databaseService.InsertApplications(dbApps);
            databaseService.InsertMessages(dbMess);

            foreach(var app in dbApps)
            {
                var messages = databaseService.GetMessagesForApplication(app.id);
                foreach(var mess in messages)
                {
                    mess.appid.Should().Be(app.id);
                }
            }
        }

        [TestMethod]
        public void GetMessagePerNewAppTest()
        {
            var fakeDB = Helper.GenerateAppsWithMessages();
            var dbApps = fakeDB.Item1;
            var dbMess = fakeDB.Item2;

            var databaseService = Helper.BuildDbService();

            databaseService.InsertApplications(dbApps);
            databaseService.InsertMessages(dbMess); 

            var messages = databaseService.GetNewMessagesForApplication(dbApps[0].id, dbMess[1].id);
            messages.Count.Should().Be(1);
        }

        [TestMethod]
        public void DeleteMessageTest()
        {
            var fakeDB = Helper.GenerateAppsWithMessages();
            var dbApps = fakeDB.Item1;
            var dbMess = fakeDB.Item2;

            var databaseService = Helper.BuildDbService();

            databaseService.InsertApplications(dbApps);
            databaseService.InsertMessages(dbMess);

            foreach(var message in dbMess)
            {
                databaseService.DeleteMessage(message);
            }

            var messages = databaseService.GetMessagesForApplication(dbMess[0].appid);
            messages.Count.Should().Be(0);
        }

        [TestMethod]
        public void DeleteAppTest()
        {
            //TODO add check for deleting messages
            var fakeDB = Helper.GenerateAppsWithMessages();
            var dbApps = fakeDB.Item1;
            var dbMess = fakeDB.Item2;

            var databaseService = Helper.BuildDbService();

            databaseService.InsertApplications(dbApps);
            databaseService.InsertMessages(dbMess);

            databaseService.DeleteApplication(dbApps[0]);

            var app = databaseService.GetApplications();
            app.Count.Should().Be(2);
        }
    }
}