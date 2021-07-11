using GotifyDesktop.New.Models;
using gotifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.New.External
{
    public class GotifySharpFactory
    {
        public GotifySharp CreateGotifySharp(GotifyServer server)
        {
            return new GotifySharp(server.Url, server.ClientToken);
        }
    }
}