using GotifyDesktop.Models;

namespace GotifyDesktop.Interfaces
{
    public interface ISettingsService
    {
        bool DoesSettingsExist();
        bool IsServerConfigured();
        ServerInfo GetSettings();
        void SaveSettings(ServerInfo serverInfo);
    }
}