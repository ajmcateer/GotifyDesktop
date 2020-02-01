using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Models
{
    public class Messagea
    {
        public int id { get; set; }
        public int appid { get; set; }
        public string message { get; set; }
        public string title { get; set; }
        public int priority { get; set; }
        //public string[] extras { get; set; }
        public DateTime date { get; set; }
    }
}
