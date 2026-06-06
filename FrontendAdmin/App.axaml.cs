using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels;
using FrontendAdmin.ViewModels.Login;
using FrontendAdmin.Views;
using Microsoft.Extensions.DependencyInjection;

namespace FrontendAdmin;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ServiceCollection serviceCollection = new();

        serviceCollection.AddCommonServices();
        serviceCollection.AddPageServices();
        serviceCollection.AddFormServices();

        ServiceProvider services = serviceCollection.BuildServiceProvider();
        MainWindowViewModel main = services.GetRequiredService<MainWindowViewModel>();
        main.CurrentPage = services.GetRequiredService<LoginPageViewModel>();

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindowView
                {
                    DataContext = main
                };
                break;
            case ISingleViewApplicationLifetime single:
                single.MainView = new MainWindowView
                {
                    DataContext = main
                };
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}