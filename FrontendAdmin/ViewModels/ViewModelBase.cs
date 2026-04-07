using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
    protected ServiceProvider Services;

    protected ViewModelBase(ServiceProvider services)
    {
        Services = services;
    }
}