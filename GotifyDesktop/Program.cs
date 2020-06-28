using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using AutofacSerilogIntegration;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Service;
using GotifyDesktop.ViewModels;
using GotifyDesktop.Views;
using gotifySharp;
using Microsoft.EntityFrameworkCore;
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

            builder.Register(c =>
            {
                DbContextOptionsBuilder<DbContext> options = new DbContextOptionsBuilder<DbContext>();
                options.UseSqlite("Data Source=gotifyDesktop.db");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                return options;
            }).AsSelf();

            //builder.RegisterInstance(options).As<DbContextOptionsBuilder<DbContext>>();
            builder.RegisterLogger();
            builder.RegisterType<AddServerViewModel>();
            builder.RegisterType<OptionsViewModel>();
            builder.RegisterType<SettingsViewModel>();
            builder.RegisterType<DatabaseContextFactory>();
            builder.RegisterType<DatabaseService>().As<IDatabaseService>();
            builder.RegisterType<GotifyServiceFactory>().As<IGotifyServiceFactory>();
            builder.RegisterType<GotifySharp>();
            builder.RegisterType<NoSyncService>().As<ISyncService>();
            builder.RegisterType<BusyViewModel>();
            builder.RegisterType<AlertMessageViewModel>();
            builder.RegisterType<MainWindowViewModel>();
            var container = builder.Build();

            window = new MainWindow
            {
                DataContext = container.Resolve<MainWindowViewModel>()
            };

            ThemeService.Initialize();

            app.Run(window);
           
        }
    }
}
