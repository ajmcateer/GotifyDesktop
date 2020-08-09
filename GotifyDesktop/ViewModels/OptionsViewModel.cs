using GotifyDesktop.Interfaces;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class OptionsViewModel : ViewModelBase, ISettingsPageInterface, IRoutableViewModel
    {
        public ObservableCollection<string> Themes { get; set; }

        public IScreen HostScreen { get; }

        public string SelectedTheme { get; set; }

        public string UrlPathSegment => throw new NotImplementedException();

        public OptionsViewModel()
        {
            HostScreen = Locator.Current.GetService<IScreen>();

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