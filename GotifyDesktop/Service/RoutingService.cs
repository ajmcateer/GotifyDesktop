using GotifyDesktop.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Service
{
    public class RoutingService
    {
        public event EventHandler<string> NavigationEvent;
        public RoutingState router { get; set; }

        public RoutingService()
        {
            
        }

        public void Init(RoutingState routingState)
        {
            router = routingState;
        }

        public void NavigateToSettings()
        {
            router.Navigate.Execute();
        }

        public void Navigate(string screen)
        {
            //NavigationEvent?.Invoke(this, screen);
        }

        public void Back()
        {
            //NavigationEvent?.Invoke((ViewModelBase)navigationStack.Pop());
        }
    }
}
