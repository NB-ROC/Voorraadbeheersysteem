using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using FrontendAdmin.Services;
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

            services.AddSingleton<DashboardPageViewModel>();
            services.AddSingleton<ProductPageViewModel>();
            services.AddSingleton<LoanPageViewModel>();
            services.AddSingleton<ProfilePageViewModel>();
            services.AddSingleton<ReservationPageViewModel>();
            services.AddSingleton<ProductFormViewModel>();

            services.AddSingleton<INavigationService>(sp => new NavigationService(
                new Dictionary<Page, Func<object>>
                {
                    [Page.Dashboard]     = () => sp.GetRequiredService<DashboardPageViewModel>(),
                    [Page.Products]      = () => sp.GetRequiredService<ProductPageViewModel>(),
                    [Page.Loans]         = () => sp.GetRequiredService<LoanPageViewModel>(),
                    [Page.Profile]       = () => sp.GetRequiredService<ProfilePageViewModel>(),
                    [Page.Reservation]   = () => sp.GetRequiredService<ReservationPageViewModel>(),
                    [Page.ProductForm]   = () => sp.GetRequiredService<ProductFormViewModel>(),
                }
            ));
            
            services.AddSingleton<MainWindowViewModel>();
            

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