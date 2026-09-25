using System;
using FrontendLender.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FrontendLender.Services;

public static class ServiceExtensions
{
    extension(IServiceCollection collection)
    {
        public void AddCommonServices()
        {
            collection
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<ISmartCardService, SmartCardService>()

                // This is the function implementation that gets passed into the NavigationService
                .AddSingleton<Func<Type, ViewModelBase?>>(provider =>
                    type => provider.GetRequiredService(type) as ViewModelBase)
                .AddSingleton<INavigationService, NavigationService>();
        }
    }
}