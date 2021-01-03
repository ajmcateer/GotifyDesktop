using GotifyDesktop.Models;
using GotifyDesktop.ViewModels;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GotifyDesktop.Service
{
    public class SettingsService
    {
        private string Path = "settings.conf";

        public SettingsService()
        {
        }

        public bool DoesSettingsExist()
        {
            if (File.Exists(Path))
            {
                return true;
            }
            return false;
        }

        public void SaveSettings(ServerInfo serverInfo)
        {
            string json = JsonConvert.SerializeObject(serverInfo);
            File.WriteAllText(Path, json);
        }

        public ServerInfo GetSettings()
        {
            if (DoesSettingsExist())
            {
                return JsonConvert.DeserializeObject<ServerInfo>(File.ReadAllText(Path));
            }
            else
            {
                return new ServerInfo();
            }
        }

        internal bool IsServerConfigured()
        {
            if (DoesSettingsExist())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
