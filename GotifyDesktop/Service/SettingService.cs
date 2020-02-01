using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GotifyDesktop.Service
{
    public class SettingService
    {
        private string Path = "settings.conf";

        public bool DoesSettingsExist()
        {
            if (File.Exists(Path))
            {
                return true;
            }
            return false;
        }

        public void SaveSettings()
        {

        }

        public void GetSettings()
        {

        }
    }
}
