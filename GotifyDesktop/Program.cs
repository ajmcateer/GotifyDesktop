using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using AutofacSerilogIntegration;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
//using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Service;
using GotifyDesktop.ViewModels;
using GotifyDesktop.Views;
using gotifySharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Splat;

namespace GotifyDesktop
{
    class Program
    {
        public static LoggingLevelSwitch loggingLevelSwitch;

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        //public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);


        // The entry point. Things aren't ready yet, so at this point
        // you shouldn't use any Avalonia types or anything that expects
        // a SynchronizationContext to be ready
        public static int Main(string[] args)
          => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        //public static AppBuilder BuildAvaloniaApp()
        //    => AppBuilder.Configure<App>()
        //        .UsePlatformDetect()
        //        .LogToDebug()
        //        .UseReactiveUI();

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
            //var builder = new ContainerBuilder();
            //loggingLevelSwitch = new LoggingLevelSwitch();
            //loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;

            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.ControlledBy(loggingLevelSwitch)
            //    .WriteTo.Console()
            //    .WriteTo.File("logs/GotifyDesktop.log", rollingInterval: RollingInterval.Day)
            //    .CreateLogger();

            //// Register individual components

            //builder.Register(c =>
            //{
            //    DbContextOptionsBuilder<DbContext> options = new DbContextOptionsBuilder<DbContext>();
            //    options.UseSqlite("Data Source=gotifyDesktop.db");
            //    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            //    return options;
            //}).AsSelf();

            ////builder.RegisterInstance(options).As<DbContextOptionsBuilder<DbContext>>();
            //builder.RegisterLogger();
            //builder.RegisterType<MainWindowViewModel>();
            //builder.RegisterType<AddServerViewModel>();
            //builder.RegisterType<OptionsViewModel>();
            //builder.RegisterType<SettingsViewModel>();
            //builder.RegisterType<DatabaseContextFactory>();
            //builder.RegisterType<DatabaseService>().As<IDatabaseService>();
            //builder.RegisterType<GotifyServiceFactory>().As<IGotifyServiceFactory>();
            //builder.RegisterType<GotifySharp>();
            //builder.RegisterType<NoSyncService>().As<ISyncService>();
            //builder.RegisterType<BusyViewModel>();
            //builder.RegisterType<AlertMessageViewModel>();
            //builder.RegisterType<RoutingService>().SingleInstance();
            ////builder.Register(c => new RoutingService() { router = c.ResolveOptional<MainWindowViewModel>().Router }).SingleInstance();
            //var container = builder.Build();

            //var test = Locator.Current.GetService<IScreen>();
            //window = new MainWindow
            //{
            //    //DataContext = Locator.Current.GetService<IScreen>()
            //    DataContext = container.Resolve<MainWindowViewModel>()
            //};

            //ThemeService.Initialize();

            //app.Run(window);
           
        }
    }
}
