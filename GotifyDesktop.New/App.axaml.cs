using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using Avalonia.ThemeManager;
using DesktopNotifications.FreeDesktop;
using DesktopNotifications.Windows;
using GotifyDesktop.New.External;
using GotifyDesktop.New.Services;
using GotifyDesktop.New.Settings;
using GotifyDesktop.New.ViewModels;
using GotifyDesktop.New.Views;
using gotifySharp;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace GotifyDesktop.New
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public IServiceProvider Container { get; private set; }
        IServiceCollection services;

        private static DesktopNotifications.INotificationManager CreateManager()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new FreeDesktopNotificationManager();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsNotificationManager();
            }

            throw new PlatformNotSupportedException();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            services = new ServiceCollection();
            var resolver = Locator.CurrentMutable;
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            ConfigureServices(services);

            Container = services.BuildServiceProvider();
            Container.UseMicrosoftDependencyResolver();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow { DataContext = Locator.Current.GetService<IScreen>() };
            }

            base.OnFrameworkInitializationCompleted();
        }

        void ConfigureServices(IServiceCollection services)
        {
            // register your personal services here, for example
            services.AddSingleton<MainWindowViewModel>(); //Implements IScreen

            // this passes IScreen resolution through to the previous viewmodel registration.
            // this is to prevent multiple instances by mistake.
            services.AddSingleton<IScreen, MainWindowViewModel>(x => x.GetRequiredService<MainWindowViewModel>());
            services.AddSingleton<IViewFor<MainWindowViewModel>, MainWindow>();
            services.AddSingleton<DesktopNotifications.INotificationManager>(CreateManager());

            //alternatively search assembly for `IRoutedViewFor` implementations
            //see https://reactiveui.net/docs/handbook/routing to learn more about routing in RxUI
            services.AddTransient<IViewFor<ServerViewModel>, ServerView>();
            services.AddTransient<ServerViewModel>();
            
            services.AddTransient<IViewFor<AddServerViewModel>, AddServerView>();
            services.AddTransient<AddServerViewModel>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<INotificationServerFactory, NotificationServerFactory>();
            services.AddTransient<RoutingState, RoutingState>();
            services.AddTransient<GotifySharpFactory, GotifySharpFactory>();
            services.UseMicrosoftDependencyResolver();
        }
    }
}