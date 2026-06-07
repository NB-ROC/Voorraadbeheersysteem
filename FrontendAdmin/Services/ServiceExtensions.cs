using System;
using FrontendAdmin.ViewModels;
using FrontendAdmin.ViewModels.Components;
using FrontendAdmin.ViewModels.Dashboard;
using FrontendAdmin.ViewModels.Loan;
using FrontendAdmin.ViewModels.Login;
using FrontendAdmin.ViewModels.Notifications;
using FrontendAdmin.ViewModels.Product;
using FrontendAdmin.ViewModels.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FrontendAdmin.Services;

public static class ServiceExtensions
{
    extension(IServiceCollection collection)
    {
        public void AddCommonServices()
        {
            collection
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<IApiService, ApiService>()

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
                .AddSingleton<LoginPageViewModel>()
                .AddSingleton<DashboardPageViewModel>()
                .AddSingleton<UserPageViewModel>()
                .AddSingleton<NotificationPageViewModel>()
                .AddSingleton<LoanPageViewModel>()
                .AddSingleton<ProductPageViewModel>();
        }

        public void AddFormServices()
        {
            collection.TryAddSingleton<HeaderViewModel>();
            collection.TryAddSingleton<FooterViewModel>();

            collection
                .AddSingleton<ProductFormViewModel>()
                .AddSingleton<NotificationDetailViewModel>()
                .AddSingleton<UserFormViewModel>();
        }
    }
}