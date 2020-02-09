using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GotifyDesktop.Views
{
    public class AlertMessageView : UserControl
    {
        public AlertMessageView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
