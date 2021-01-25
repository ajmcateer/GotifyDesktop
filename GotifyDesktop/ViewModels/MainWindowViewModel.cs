using GotifyDesktop.Infrastructure;
using Serilog;
using ReactiveUI;
using System.Threading.Tasks;
using System.Reactive.Linq;
using GotifyDesktop.Interfaces;
using System.Reactive.Disposables;
using System;

namespace GotifyDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IActivatableViewModel, IScreen
    {
        public ViewModelActivator Activator { get; }

        ServerViewModelFactory _serverViewModelFactory;
        public RoutingState Router { get; } 

        public MainWindowViewModel(ServerViewModelFactory serverViewModelFactory,
            RoutingState routingState)
        {
            _serverViewModelFactory = serverViewModelFactory;
            Router = routingState;
            Activator = new ViewModelActivator();

            this.WhenActivated(async (CompositeDisposable disposables) =>
            {
                await OnActivationAsync();
                Disposable
                    .Create(() => { /* handle deactivation */ })
                    .DisposeWith(disposables);
            });
        }

        public async Task OnActivationAsync()
        {
            var serverViewModel =_serverViewModelFactory.GetNewServerViewModel(this as IScreen);
            await Router.Navigate.Execute(serverViewModel);
        }
    }
}