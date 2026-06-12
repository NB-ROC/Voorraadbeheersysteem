using System;
using Frontend.ViewModels;
using Frontend.ViewModels.Components;
using Frontend.ViewModels.CustomerProduct;
using Frontend.ViewModels.Dashboard;
using Frontend.ViewModels.Loan;
using Frontend.ViewModels.Login;
using Frontend.ViewModels.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Frontend.Services;

public static class ServiceExtensions
{
    extension(IServiceCollection collection)
    {
        public void AddCommonServices()
        {
            collection
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<IApiService, ApiService>()
                .AddSingleton<ISmartCardService, SmartCardService>()

                // This is the function implementation that gets passed into the NavigationService
                .AddSingleton<Func<Type, ViewModelBase?>>(provider =>
                    type => provider.GetRequiredService(type) as ViewModelBase)
                .AddSingleton<INavigationService, NavigationService>();
        }

        public void AddPageServices()
        {
            collection.TryAddSingleton<HeaderViewModel>();
            collection.TryAddSingleton<FooterViewModel>();

            collection
                .AddSingleton<LoginScannerPageViewModel>()
                .AddSingleton<LoginPageViewModel>()
                .AddSingleton<CustomerProductPageViewModel>()
                .AddSingleton<DashboardPageViewModel>()
                .AddSingleton<UserPageViewModel>()
                .AddSingleton<LoanPageViewModel>();
        }

        public void AddFormServices()
        {
            collection.TryAddSingleton<HeaderViewModel>();
            collection.TryAddSingleton<FooterViewModel>();

            collection
                .AddSingleton<UserFormViewModel>();
        }
    }
}