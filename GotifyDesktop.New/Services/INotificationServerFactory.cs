using GotifyDesktop.New.Models;
using GotifyDesktop.New.ViewModels;

namespace GotifyDesktop.New.Services
{
    public interface INotificationServerFactory
    {
        ServerViewModel GenerateNewView(GotifyServer server);
    }
}