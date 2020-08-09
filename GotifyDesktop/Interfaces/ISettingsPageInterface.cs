using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Interfaces
{
    public interface ISettingsPageInterface
    {
        public Dictionary<string,string> Save();
    }
}
