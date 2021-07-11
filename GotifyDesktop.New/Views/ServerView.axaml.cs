using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GotifyDesktop.New.ViewModels;

namespace GotifyDesktop.New.Views
{
    public partial class ServerView : ReactiveUserControl<ServerViewModel>
    {
        public ServerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
