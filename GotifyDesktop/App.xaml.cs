using Autofac;
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
        public static IContainer Container; 

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
            Container = BuildContainer();

            mainWindowViewModel = Container.Resolve<MainWindowViewModel>();

            Locator.CurrentMutable.RegisterConstant<IScreen>(mainWindowViewModel);
            Locator.CurrentMutable.Register<IViewFor<OptionsViewModel>>(() => new OptionsView());
            Locator.CurrentMutable.Register<IViewFor<SettingsViewModel>>(() => new SettingsView());
            Locator.CurrentMutable.Register<IViewFor<ServerViewModel>>(() => new ServerView());

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow { DataContext = Locator.Current.GetService<IScreen>() };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            //loggingLevelSwitch = new LoggingLevelSwitch();
            //loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;

            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.ControlledBy(loggingLevelSwitch)
            //    .WriteTo.Console()
            //    .WriteTo.File("logs/GotifyDesktop.log", rollingInterval: RollingInterval.Day)
            //    .CreateLogger();

            builder.Register(ctx =>
            {
                return new RoutingState();
            }).As<RoutingState>();
            //builder.RegisterLogger();
            builder.RegisterType<MainWindowViewModel>();
            builder.RegisterType<AddServerViewModel>();
            builder.RegisterType<OptionsViewModel>();
            builder.RegisterType<ServerViewModelFactory>();
            //TODO: Add fileservice to determine save location per OS and inject into SettingsService
            builder.RegisterType<SettingsService>()
                .WithParameter(new TypedParameter(typeof(string), "settings.conf")).As<ISettingsService>();
            builder.RegisterType<ViewModelActivator>();
            builder.RegisterType<GotifyServiceFactory>().As<IGotifyServiceFactory>();
            builder.RegisterType<GotifyServiceFactory>();
            builder.RegisterType<SettingsViewModel>();
            builder.RegisterType<BusyViewModel>();
            builder.RegisterType<AlertMessageViewModel>();
            return builder.Build();
        }
    }
}
