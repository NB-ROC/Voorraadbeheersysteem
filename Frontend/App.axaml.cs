using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Frontend.Services;
using Frontend.ViewModels;
using Frontend.ViewModels.CustomerProduct;
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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ServiceCollection serviceCollection = new();

            MainWindowViewModel mainWindowViewModel = new();
            NavigationService navigationService = new(mainWindowViewModel);
            BackendService backendService = new();

            serviceCollection.AddSingleton(navigationService);
            serviceCollection.AddSingleton(backendService);

            _ = backendService.LogIn("testmail@roc-nijmegen.nl", "Placeholder1");


            DisableAvaloniaDataAnnotationValidation();

            ServiceProvider services = serviceCollection.BuildServiceProvider();


            mainWindowViewModel.CurrentPage = new CustomerProductPageViewModel(services);

            desktop.MainWindow = new MainWindowView
            {
                DataContext = mainWindowViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        DataAnnotationsValidationPlugin[] dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (DataAnnotationsValidationPlugin plugin in dataValidationPluginsToRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }
}