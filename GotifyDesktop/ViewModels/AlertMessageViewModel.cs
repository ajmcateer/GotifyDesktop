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

        public int RetryCount
        {
            get => retryCount;
            set => this.RaiseAndSetIfChanged(ref retryCount, value);
        }

        public bool IsDisplayVisible
        {
            get => isDisplayVisible;
            set
            {
                if(value == true)
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
            RetryCount = 10;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RetryCount--;
            if(RetryCount == 0)
            {
                Retry?.Invoke(this, null);
                RetryCount = 10;
            }
        }

        public void RetryConnection()
        {
            Retry?.Invoke(this, null);
        }
    }
}
