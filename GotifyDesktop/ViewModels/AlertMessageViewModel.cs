using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.ViewModels
{
    public class AlertMessageViewModel : ViewModelBase
    {
        public event EventHandler Retry;

        DispatcherTimer timer;
        bool isDisplayVisible;
        int retryCount;
        string retryMessage;

        public string RetryMessage
        {
            get => retryMessage;
            set => this.RaiseAndSetIfChanged(ref retryMessage, value);
        }

        public bool IsDisplayVisible
        {
            get => isDisplayVisible;
            set
            {
                if(value == true && timer.IsEnabled == false)
                {
                    timer.Start();
                }
                else if (value == false)
                {
                    timer.Stop();
                }
                this.RaiseAndSetIfChanged(ref isDisplayVisible, value);
            }
        }

        public AlertMessageViewModel()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            retryCount = 10;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            retryCount--;
            RetryMessage = $"Failed to Connect retrying in {retryCount}";
            if(retryCount == 0)
            {
                Retry?.Invoke(this, null);
                retryCount = 10;
            }
        }

        public void RetryConnection()
        {
            Retry?.Invoke(this, null);
        }
    }
}
