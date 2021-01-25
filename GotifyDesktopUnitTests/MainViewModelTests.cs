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
    public class MainWindowViewModelTests
    {
        [TestMethod]
        public void ShouldNavigateToServerView()
        {
            var GotifyServiceFactory = new Mock<GotifyServiceFactory>();
            var AddNewServerVm = new Mock<AddServerViewModel>();
            var OptionsVm = new Mock<OptionsViewModel>();
            var ISettingService = new Mock<ISettingsService>();
            var SettingVm = new Mock<SettingsViewModel>(AddNewServerVm.Object, OptionsVm.Object, ISettingService.Object);
            var ServerVmFactory = new Mock<ServerViewModelFactory>(GotifyServiceFactory.Object, SettingVm.Object);

            var mainVm = new MainWindowViewModel(ServerVmFactory.Object, new RoutingState());
            mainVm.Activator.Activate();

            var result = (ServerViewModel)mainVm.Router.GetCurrentViewModel();

            result.Should().BeOfType<ServerViewModel>();
        }
    }
}
