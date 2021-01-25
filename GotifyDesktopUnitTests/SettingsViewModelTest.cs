using FluentAssertions;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Interfaces;
using GotifyDesktop.Models;
using GotifyDesktop.Service;
using GotifyDesktop.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktopUnitTests
{
    [TestClass]
    public class SettingsViewModelTest
    {

        [TestMethod]
        public void WithOutServerConfigured()
        {
            var GotifyServiceFactory = new Mock<GotifyServiceFactory>();
            var AddNewServerVm = new Mock<AddServerViewModel>();

            var OptionsVm = new Mock<OptionsViewModel>();
            var ISettingService = new Mock<ISettingsService>();

            ISettingService.Setup(moq_sSer => moq_sSer.IsServerConfigured()).Returns(false);

            var SettingVm = new SettingsViewModel(AddNewServerVm.Object, OptionsVm.Object, ISettingService.Object);
            SettingVm.Activator.Activate();

            AddNewServerVm.Verify(asVm => asVm.SetNewServer(), Times.Once);
        }

        [TestMethod]
        public void WithServerConfigured()
        {
            var GotifyServiceFactory = new Mock<GotifyServiceFactory>();
            var AddNewServerVm = new Mock<AddServerViewModel>();

            var OptionsVm = new Mock<OptionsViewModel>();
            var ISettingService = new Mock<ISettingsService>();

            var serverInfo = new ServerInfo();

            ISettingService.Setup(moq_sSer => moq_sSer.IsServerConfigured()).Returns(true);
            ISettingService.Setup(moq_sSer => moq_sSer.GetSettings()).Returns(serverInfo);

            var SettingVm = new SettingsViewModel(AddNewServerVm.Object, OptionsVm.Object, ISettingService.Object);
            SettingVm.Activator.Activate();

            AddNewServerVm.Verify(asVm => asVm.SetServerInfo(serverInfo), Times.Once);
        }
    }
}
