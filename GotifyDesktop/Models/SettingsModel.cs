using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GotifyDesktop.Models
{
    public class SettingsModel
    {
        [Key]
        public String Key { get; set; }

        public String Value { get; set; }
    }
}
