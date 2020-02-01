using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GotifyDesktop.Views
{
    public class MainControlView : UserControl
    {
        public MainControlView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
