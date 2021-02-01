using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.Interfaces
{
    interface ICustomScreen: IScreen
    {
        public Task NavigateToSettings();
    }
}
