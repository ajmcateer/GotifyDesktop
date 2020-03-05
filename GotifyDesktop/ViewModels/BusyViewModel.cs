using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.ViewModels
{
    public class BusyViewModel : ViewModelBase
    {
        bool isVisible;

        public bool IsVisible
        {
            get => isVisible;
            set => this.RaiseAndSetIfChanged(ref isVisible, value);
        }

        public void Show()
        {
            IsVisible = true;
        }

        public void Close()
        {
            IsVisible = false;
        }
    }
}

