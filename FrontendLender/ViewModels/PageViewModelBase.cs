using System.Threading.Tasks;

namespace FrontendLender.ViewModels;

public abstract class PageViewModelBase : ViewModelBase
{
    // public PageViewModelBase(HeaderViewModel header, FooterViewModel footer)
    // {
    //     Header = header;
    //     Footer = footer;
    // }
    //
    // public HeaderViewModel Header { get; set; }
    // public FooterViewModel Footer { get; set; }

    public abstract Task LoadAsync();
}