using System.Threading.Tasks;
using Frontend.ViewModels.Components;

namespace Frontend.ViewModels;

public abstract class FormViewModelBase<TModel> : ViewModelBase
    where TModel : class
{
    protected FormViewModelBase(HeaderViewModel header, FooterViewModel footer)
    {
        Header = header;
        Footer = footer;
    }

    public HeaderViewModel Header { get; set; }
    public FooterViewModel Footer { get; set; }

    public abstract Task LoadAsync(TModel? existing);
}