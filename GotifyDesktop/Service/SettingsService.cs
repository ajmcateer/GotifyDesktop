using GotifyDesktop.Interfaces;
using GotifyDesktop.Models;
using Newtonsoft.Json;
using System.IO;

namespace GotifyDesktop.Service
{
    public class SettingsService : ISettingsService
    {
        private string _path;

        public SettingsService(string path)
        {
            _path = path;
        }

        public bool DoesSettingsExist()
        {
            if (File.Exists(_path))
            {
                return true;
            }
            return false;
        }

        public void SaveSettings(ServerInfo serverInfo)
        {
            string json = JsonConvert.SerializeObject(serverInfo);
            File.WriteAllText(_path, json);
        }

        public ServerInfo GetSettings()
        {
            if (DoesSettingsExist())
            {
                return JsonConvert.DeserializeObject<ServerInfo>(File.ReadAllText(_path));
            }
            else
            {
                return new ServerInfo();
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
