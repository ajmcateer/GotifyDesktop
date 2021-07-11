using GotifyDesktop.New.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.New.Settings
{
    public class SettingsService : ISettingsService
    {
        private string _path;

        public SettingsService()
        {
            _path = "settings.cnf";
        }

        public void DeleteServer()
        {
            File.Delete(_path);
        }

        public bool DoesSettingsExist()
        {
            if (File.Exists(_path))
            {
                return true;
            }
            return false;
        }

        public void SaveSettings(GotifyServer serverInfo)
        {
            string json = JsonConvert.SerializeObject(serverInfo);
            File.WriteAllText(_path, json);
        }

        public GotifyServer GetSettings()
        {
            if (DoesSettingsExist())
            {
                return JsonConvert.DeserializeObject<GotifyServer>(File.ReadAllText(_path));
            }
            else
            {
                return new GotifyServer();
            }
        }

        public bool IsServerConfigured()
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
