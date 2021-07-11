using GotifyDesktop.New.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.New.Settings
{
    public interface ISettingsService
    {
        bool DoesSettingsExist();
        bool IsServerConfigured();
        GotifyServer GetSettings();
        void SaveSettings(GotifyServer serverInfo);
        void DeleteServer();
    }
}