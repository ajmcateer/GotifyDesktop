using GotifyDesktop.Service;
using Serilog;

namespace GotifyDesktop.Infrastructure
{
    public interface IGotifyServiceFactory
    {
        IGotifyService CreateNewGotifyService(ILogger ilogger);
    }
}