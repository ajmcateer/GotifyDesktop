using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Interfaces
{
    interface ICustomScreen: IScreen
    {
        public void NavigateToSettings();
    }
}
