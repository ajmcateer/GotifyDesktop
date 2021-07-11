using FluentAssertions;
using GotifyDesktop.New.External;
using GotifyDesktop.New.ViewModels;
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
            var gotifysharp = new Mock<GotifySharpFactory>();

            //var mainVm = new MainWindowViewModel();
            //mainVm.Activator.Activate();

            //var result = (ServerViewModel)mainVm.Router.GetCurrentViewModel();

            //result.Should().BeOfType<ServerViewModel>();
        }
    }
}
