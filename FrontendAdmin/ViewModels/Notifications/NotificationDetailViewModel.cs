using System.Reactive;
using System.Threading.Tasks;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Components;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Notifications;

public class NotificationDetailViewModel : FormViewModelBase<NotificationModel>
{
    private readonly INavigationService _navigation;
    
    public NotificationDetailViewModel(HeaderViewModel header, FooterViewModel footer, INavigationService navigation) : base(header, footer)
    {
        _navigation = navigation;
    
        BackCommand = ReactiveCommand.CreateFromTask(GoBack);
    }
    
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ReactiveCommand<Unit, Unit> BackCommand { get; }
    
    private async Task GoBack()
    {
        await _navigation.NavigateTo<NotificationPageViewModel>();
    }
    
    public override async Task LoadAsync(NotificationModel? notification)
    {
        if (notification is null)
        {
            await GoBack();
            return;
        }
        
        Title = notification.Title;
        Description = notification.Description;
    }
}

