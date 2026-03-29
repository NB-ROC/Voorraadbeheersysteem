using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using FrontendAdmin.ViewModels;
using FrontendAdmin.ViewModels.Dashboard;
using FrontendAdmin.ViewModels.Loan;
using FrontendAdmin.ViewModels.Product;
using FrontendAdmin.ViewModels.Profile;
using FrontendAdmin.ViewModels.Reservation;
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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ServiceCollection services = new();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<ProductPageViewModel>();
            services.AddSingleton<DashboardPageViewModel>();
            services.AddSingleton<LoanPageViewModel>();
            services.AddSingleton<ReservationPageViewModel>();
            services.AddSingleton<ProfilePageViewModel>();


            DisableAvaloniaDataAnnotationValidation();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            MainWindowViewModel mainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();

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