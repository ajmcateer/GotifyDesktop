using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using AutofacSerilogIntegration;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Service;
using GotifyDesktop.ViewModels;
using GotifyDesktop.Views;
using gotifySharp;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace GotifyDesktop
{
    class Program
    {
        public static LoggingLevelSwitch loggingLevelSwitch;

        public static MainWindow window;

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
            var builder = new ContainerBuilder();
            loggingLevelSwitch = new LoggingLevelSwitch();
            loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                .WriteTo.Console()
                .WriteTo.File("logs/GotifyDesktop.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Register individual components
            builder.RegisterType<DatabaseContextFactory>();
            builder.RegisterType<GotifyServiceFactory>();
            builder.RegisterLogger();
            builder.RegisterType<GotifySharp>();
            builder.RegisterType<GotifyService>();
            builder.RegisterType<GotifyService>().Named<GotifyService>("TestService");
            builder.RegisterType<DatabaseService>();
            builder.RegisterType<SyncService>().SingleInstance();
            var container = builder.Build();

            window = new MainWindow
            {
                DataContext = new MainWindowViewModel(container),
            };

            app.Run(window);
        }
    }
}
