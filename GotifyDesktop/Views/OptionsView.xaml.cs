using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GotifyDesktop.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace GotifyDesktop.Views
{
    public class OptionsView : ReactiveUserControl<OptionsViewModel>
    {
        public OptionsView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.WhenActivated((CompositeDisposable disposable) => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
