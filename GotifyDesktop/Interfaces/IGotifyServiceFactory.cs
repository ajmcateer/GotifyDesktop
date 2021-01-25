using GotifyDesktop.Models;
using GotifyDesktop.Service;
using Serilog;

namespace GotifyDesktop.Infrastructure
{
    public interface IGotifyServiceFactory
    {
        IGotifyService CreateNewGotifyService(ServerInfo serverInfo);
    }
}