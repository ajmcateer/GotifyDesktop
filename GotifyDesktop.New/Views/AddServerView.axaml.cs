using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GotifyDesktop.New.ViewModels;

namespace GotifyDesktop.New.Views
{
    public partial class AddServerView : ReactiveUserControl<AddServerViewModel>
    {
        public AddServerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}