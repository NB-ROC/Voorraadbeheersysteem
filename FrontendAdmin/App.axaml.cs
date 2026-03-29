using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using FrontendAdmin.ViewModels;
using FrontendAdmin.ViewModels.Dashboard;
using FrontendAdmin.ViewModels.Loan;
using Microsoft.Extensions.DependencyInjection;
using FrontendAdmin.ViewModels.Product;
using FrontendAdmin.ViewModels.Profile;
using FrontendAdmin.ViewModels.Reservation;
using FrontendAdmin.Views;

namespace FrontendAdmin;

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
            var services = new ServiceCollection();
    
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<ProductPageViewModel>();
            services.AddSingleton<DashboardPageViewModel>();
            services.AddSingleton<LoanPageViewModel>();
            services.AddSingleton<ReservationPageViewModel>();
            services.AddSingleton<ProfilePageViewModel>();

            
            DisableAvaloniaDataAnnotationValidation();

            var serviceProvider = services.BuildServiceProvider();

            var mainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();

            desktop.MainWindow = new MainWindow
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