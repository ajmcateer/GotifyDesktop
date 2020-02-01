using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GotifyDesktop.Views
{
    public class MainControlv2View : UserControl
    {
        public MainControlv2View()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
