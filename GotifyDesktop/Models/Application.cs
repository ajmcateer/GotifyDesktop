using Avalonia.Diagnostics.ViewModels;
using GotifyDesktop.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Models
{
    public class Applicationa
    {
        public string name { get; set; }
        public int id { get; set; }
        public string token { get; set; }
        public string description { get; set; }
        public bool _internal { get; set; }
        public string image { get; set; }
    }
}
