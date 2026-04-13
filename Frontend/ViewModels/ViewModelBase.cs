using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
    protected readonly ServiceProvider Services;

    protected ViewModelBase(ServiceProvider services)
    {
        Services = services;
    }
}