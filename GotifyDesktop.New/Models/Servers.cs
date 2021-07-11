using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.New.Models
{
    public class Servers : ReactiveObject
    {
        private string serverName;

        public string ServerName
        {
            get => serverName;
            set => this.RaiseAndSetIfChanged(ref serverName, value);
        }
    }
}
