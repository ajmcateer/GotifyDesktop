using FluentAssertions;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Interfaces;
using GotifyDesktop.Models;
using GotifyDesktop.Service;
using GotifyDesktop.ViewModels;
using gotifySharp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using static gotifySharp.Enums.ConnectionInfo;

namespace GotifyDesktopUnitTests
{
    [TestClass]
    public class ServerViewModelTests
    {
        [TestMethod]
        public void ShouldShowSettingsScreen()
        {
            var gotifyServiceFactory = new Mock<GotifyServiceFactory>();
            var addNewServerVm = new Mock<AddServerViewModel>();
            var optionsVm = new Mock<OptionsViewModel>();
            var iSettingService = new Mock<ISettingsService>();
            iSettingService.Setup(moq_sSer => moq_sSer.IsServerConfigured()).Returns(false);

            var settingVm = new Mock<SettingsViewModel>(addNewServerVm.Object, optionsVm.Object, iSettingService.Object);
            var hostScreen = new Mock<IScreen>();
            hostScreen.Setup(screen => screen.Router).Returns(new RoutingState());

            var serverVM = new ServerViewModel(gotifyServiceFactory.Object, settingVm.Object, hostScreen.Object);
            serverVM.Activator.Activate();

            hostScreen.Verify(screen => screen.Router, Times.Once);
        }

        [TestMethod]
        public void ShouldSubscribeToDisconnect()
        {

            var gotifyService = new Mock<IGotifyService>();
            gotifyService.SetupAdd(onDC => onDC.OnDisconnect += It.IsAny<EventHandler<WebsocketDisconnectStatus>>());

            var gotifyServiceFactory = new Mock<IGotifyServiceFactory>();
            gotifyServiceFactory.Setup(fac => fac.CreateNewGotifyService(It.IsAny<ServerInfo>())).Returns(gotifyService.Object);

            var addNewServerVm = new Mock<AddServerViewModel>();
            var optionsVm = new Mock<OptionsViewModel>();
            var iSettingService = new Mock<ISettingsService>();
            iSettingService.Setup(moq_sSer => moq_sSer.IsServerConfigured()).Returns(true);

            var settingVm = new Mock<SettingsViewModel>(addNewServerVm.Object, optionsVm.Object, iSettingService.Object);
            var hostScreen = new Mock<IScreen>();
            hostScreen.Setup(screen => screen.Router).Returns(new RoutingState());

            var serverVM = new ServerViewModel(gotifyServiceFactory.Object, settingVm.Object, hostScreen.Object);
            serverVM.Activator.Activate();

            gotifyService.VerifyAdd(onDC => onDC.OnDisconnect += It.IsAny<EventHandler<WebsocketDisconnectStatus>>(), Times.Once());
        }

        [TestMethod]
        public void ShouldSubscribeToReconnect()
        {

            var gotifyService = new Mock<IGotifyService>();
            gotifyService.SetupAdd(onDC => onDC.OnReconnect += It.IsAny<EventHandler<WebsocketReconnectStatus>>());

            var gotifyServiceFactory = new Mock<IGotifyServiceFactory>();
            gotifyServiceFactory.Setup(fac => fac.CreateNewGotifyService(It.IsAny<ServerInfo>())).Returns(gotifyService.Object);

            var addNewServerVm = new Mock<AddServerViewModel>();
            var optionsVm = new Mock<OptionsViewModel>();
            var iSettingService = new Mock<ISettingsService>();
            iSettingService.Setup(moq_sSer => moq_sSer.IsServerConfigured()).Returns(true);

            var settingVm = new Mock<SettingsViewModel>(addNewServerVm.Object, optionsVm.Object, iSettingService.Object);
            var hostScreen = new Mock<IScreen>();
            hostScreen.Setup(screen => screen.Router).Returns(new RoutingState());

            var serverVM = new ServerViewModel(gotifyServiceFactory.Object, settingVm.Object, hostScreen.Object);
            serverVM.Activator.Activate();

            gotifyService.VerifyAdd(onDC => onDC.OnReconnect += It.IsAny<EventHandler<WebsocketReconnectStatus>>(), Times.Once);
        }

        [TestMethod]
        public void ShouldSubscribeToOnMessace()
        {

            var gotifyService = new Mock<IGotifyService>();
            gotifyService.SetupAdd(onDC => onDC.OnMessage += It.IsAny<EventHandler<MessageModel>>());

            var gotifyServiceFactory = new Mock<IGotifyServiceFactory>();
            gotifyServiceFactory.Setup(fac => fac.CreateNewGotifyService(It.IsAny<ServerInfo>())).Returns(gotifyService.Object);

            var addNewServerVm = new Mock<AddServerViewModel>();
            var optionsVm = new Mock<OptionsViewModel>();
            var iSettingService = new Mock<ISettingsService>();
            iSettingService.Setup(moq_sSer => moq_sSer.IsServerConfigured()).Returns(true);

            var settingVm = new Mock<SettingsViewModel>(addNewServerVm.Object, optionsVm.Object, iSettingService.Object);
            var hostScreen = new Mock<IScreen>();
            hostScreen.Setup(screen => screen.Router).Returns(new RoutingState());

            var serverVM = new ServerViewModel(gotifyServiceFactory.Object, settingVm.Object, hostScreen.Object);
            serverVM.Activator.Activate();

            gotifyService.VerifyAdd(onDC => onDC.OnMessage += It.IsAny<EventHandler<MessageModel>>(), Times.Once);
        }

        [TestMethod]
        public void ShouldHandleDisconnectEvent()
        {

            var gotifyService = new Mock<IGotifyService>();
            gotifyService.SetupAdd(onDC => onDC.OnDisconnect += It.IsAny<EventHandler<WebsocketDisconnectStatus>>());

            var gotifyServiceFactory = new Mock<IGotifyServiceFactory>();
            gotifyServiceFactory.Setup(fac => fac.CreateNewGotifyService(It.IsAny<ServerInfo>())).Returns(gotifyService.Object);

            var addNewServerVm = new Mock<AddServerViewModel>();
            var optionsVm = new Mock<OptionsViewModel>();
            var iSettingService = new Mock<ISettingsService>();
            iSettingService.Setup(moq_sSer => moq_sSer.IsServerConfigured()).Returns(true);

            var settingVm = new Mock<SettingsViewModel>(addNewServerVm.Object, optionsVm.Object, iSettingService.Object);
            var hostScreen = new Mock<IScreen>();
            hostScreen.Setup(screen => screen.Router).Returns(new RoutingState());

            var serverVM = new ServerViewModel(gotifyServiceFactory.Object, settingVm.Object, hostScreen.Object);

            gotifyService.Raise(dc => dc.OnDisconnect += null, new EventArgs());
        }
    }
}
