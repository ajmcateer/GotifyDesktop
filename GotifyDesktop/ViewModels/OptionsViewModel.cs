using GotifyDesktop.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class OptionsViewModel : ViewModelBase, ISettingsPageInterface
    {
        public ObservableCollection<string> Themes { get; set; }

        public string SelectedTheme { get; set; }

        public OptionsViewModel()
        {
            Themes = new ObservableCollection<string>();
            Themes.Add("Dark");
            Themes.Add("Light");
        }

        public Dictionary<string, string> Save()
        {
            throw new NotImplementedException();
        }
    }
}
