using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.ViewModels.Components;

namespace Frontend.ViewModels;

public abstract class SelectionViewModelBase<TSelectable> : ViewModelBase
where TSelectable : class
{
    protected SelectionViewModelBase(HeaderViewModel header, FooterViewModel footer)
    {
        Header = header;
        Footer = footer;
    }

    public HeaderViewModel Header { get; set; }
    public FooterViewModel Footer { get; set; }

    protected Action<TSelectable?>? Select;
    
    public abstract Task LoadAsync(Action<TSelectable?> select, List<TSelectable>? data);
}