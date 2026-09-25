using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Frontend.Services;
using Frontend.ViewModels;
using Frontend.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Frontend;

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

        ServiceProvider services = serviceCollection.BuildServiceProvider();
        MainWindowViewModel main = services.GetRequiredService<MainWindowViewModel>();
        INavigationService navigation = services.GetRequiredService<INavigationService>();
        navigation.NavigateTo<PAGE_NAME>().Wait();
        
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