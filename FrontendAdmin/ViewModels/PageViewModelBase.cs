using System.Threading.Tasks;
using FrontendAdmin.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;

namespace FrontendAdmin.ViewModels;

public abstract class PageViewModelBase : ViewModelBase
{
    public PageViewModelBase(HeaderViewModel header, FooterViewModel footer)
    {
        Header = header;
        Footer = footer;
    }

    public HeaderViewModel Header { get; set; }
    public FooterViewModel Footer { get; set; }

    public abstract Task LoadAsync();
}