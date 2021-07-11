using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GotifyDesktop.New.ViewModels;
using System;

namespace GotifyDesktop.New.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            this.WhenActivated(disposables => { /* Handle view activation etc. */ });

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void WhenActivated(Action<object> p)
        {
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}