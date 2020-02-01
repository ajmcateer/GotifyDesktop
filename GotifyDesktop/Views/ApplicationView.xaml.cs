using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GotifyDesktop.Views
{
    public class ApplicationView : UserControl
    {
        public ApplicationView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
