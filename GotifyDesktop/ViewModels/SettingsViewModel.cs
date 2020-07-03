using GotifyDesktop.Interfaces;
using GotifyDesktop.Service;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        TaskCompletionSource<int> taskCompletionSource;
        private bool isVisible;

        IServerPageInterface addServerViewModel;
        ISettingsPageInterface optionsViewModel;
        IDatabaseService databaseService;

        public IDatabaseService DatabaseService
        {
            get => databaseService;
            set => this.RaiseAndSetIfChanged(ref databaseService, value);
        }
        public ISettingsPageInterface OptionsViewModel
        {
            get => optionsViewModel;
            set => this.RaiseAndSetIfChanged(ref optionsViewModel, value);
        }

        public IServerPageInterface AddServerViewModel
        {
            get => addServerViewModel;
            set => this.RaiseAndSetIfChanged(ref addServerViewModel, value);
        }

        public bool IsVisible
        {
            get => isVisible;
            set => this.RaiseAndSetIfChanged(ref isVisible, value);
        }

        public SettingsViewModel(AddServerViewModel addServerViewModel, OptionsViewModel optionsViewModel, IDatabaseService databaseService)
        {
            AddServerViewModel = addServerViewModel;
            OptionsViewModel = optionsViewModel;
            DatabaseService = databaseService;
        }

        public async Task<int> ShowAsync()
        {
            IsVisible = true;
            taskCompletionSource = new TaskCompletionSource<int>();

            var result = await taskCompletionSource.Task;

            IsVisible = false;
            return 0;
        }
    }
}
