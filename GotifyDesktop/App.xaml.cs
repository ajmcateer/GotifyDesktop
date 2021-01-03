﻿using Autofac;
using AutofacSerilogIntegration;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ExtendedToolkit;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using GotifyDesktop.Infrastructure;
using GotifyDesktop.Interfaces;
using GotifyDesktop.Service;
using GotifyDesktop.ViewModels;
using GotifyDesktop.Views;
using gotifySharp;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Splat;
using System;

namespace GotifyDesktop
{
    public class App : Application
    {
        public static Styles FluentDark = new Styles
        {
            new StyleInclude(new Uri("avares://ControlCatalog/Styles"))
            {
                Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/FluentDark.xaml")
            },
        };

        public static Styles FluentLight = new Styles
        {
            new StyleInclude(new Uri("avares://ControlCatalog/Styles"))
            {
                Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/FluentLight.xaml")
            },
        };

        public static Styles DefaultLight = new Styles
        {
            new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
            {
                Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/Base.xaml")
            },
            new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
            {
                Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/BaseLight.xaml")
            },
            new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
            {
                Source = new Uri("avares://Avalonia.Themes.Default/Accents/BaseLight.xaml")
            },
            new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
            {
                Source = new Uri("avares://Avalonia.Themes.Default/DefaultTheme.xaml")
            },
        };

        public static Styles DefaultDark = new Styles
        {
            new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
            {
                Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/Base.xaml")
            },
            new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
            {
                Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/BaseDark.xaml")
            },
            new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
            {
                Source = new Uri("avares://Avalonia.Themes.Default/Accents/BaseDark.xaml")
            },
            new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
            {
                Source = new Uri("avares://Avalonia.Themes.Default/DefaultTheme.xaml")
            },
        };

        public static LoggingLevelSwitch loggingLevelSwitch;

        public static MainWindowViewModel mainWindowViewModel;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var container = BuildContainer();

            mainWindowViewModel = container.Resolve<MainWindowViewModel>();

            Locator.CurrentMutable.RegisterConstant<ICustomScreen>(mainWindowViewModel);
            Locator.CurrentMutable.Register<IViewFor<OptionsViewModel>>(() => new OptionsView());
            Locator.CurrentMutable.Register<IViewFor<SettingsViewModel>>(() => new SettingsView());
            Locator.CurrentMutable.Register<IViewFor<ServerViewModel>>(() => new ServerView());

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow { DataContext = Locator.Current.GetService<ICustomScreen>() };
                //SkinManager.Instance.EnableSkin(desktop.MainWindow);
                //ThemeManager.Instance.EnableTheme(desktop.MainWindow);
            }

            base.OnFrameworkInitializationCompleted();
        }

        private IContainer BuildContainer()
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

            //builder.RegisterInstance(options).As<DbContextOptionsBuilder<DbContext>>();
            builder.RegisterLogger();
            builder.RegisterType<MainWindowViewModel>();
            builder.RegisterType<AddServerViewModel>();
            builder.RegisterType<OptionsViewModel>();
            builder.RegisterType<ServerViewModelFactory>();
            builder.RegisterType<SettingsService>();
            builder.RegisterType<ViewModelActivator>();
            //builder.RegisterType<DatabaseContextFactory>();
            //builder.RegisterType<DatabaseService>().As<IDatabaseService>();
            builder.RegisterType<GotifyServiceFactory>().As<IGotifyServiceFactory>();
            builder.RegisterType<GotifyServiceFactory>();
            builder.RegisterType<SettingsViewModel>();
            //builder.RegisterType<GotifySharp>();
            builder.RegisterType<NoSyncService>().As<ISyncService>();
            builder.RegisterType<BusyViewModel>();
            builder.RegisterType<AlertMessageViewModel>();
            //builder.Register(c => new RoutingService() { router = c.ResolveOptional<MainWindowViewModel>().Router }).SingleInstance();
            return builder.Build();
        }
    }
}
