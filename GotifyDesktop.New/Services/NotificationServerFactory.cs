using GotifyDesktop.New.Models;
using GotifyDesktop.New.ViewModels;
using gotifySharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.New.Services
{
    public class NotificationServerFactory : INotificationServerFactory
    {
        DesktopNotifications.INotificationManager NotificationManager;

        public NotificationServerFactory(DesktopNotifications.INotificationManager notificationManager)
        {
            NotificationManager = notificationManager;
            NotificationManager.Initialize();
        }

        public ServerViewModel GenerateNewView(GotifyServer server)
        {
            var gotifySharp = new GotifySharp(server.Url, server.ClientToken);
            return new ServerViewModel(gotifySharp, NotificationManager);
        }
    }
}
