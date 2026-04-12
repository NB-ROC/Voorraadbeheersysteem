using FrontendAdmin.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;

namespace FrontendAdmin.ViewModels;

public class PageViewModelBase : ViewModelBase
{
    public PageViewModelBase(ServiceProvider services) : base(services)
    {
        Header = new HeaderViewModel(services);
        Footer = new FooterViewModel(services);
    }

    public HeaderViewModel Header { get; set; }
    public FooterViewModel Footer { get; set; }
}