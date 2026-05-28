using System.Reactive;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Notifications;

public class NotificationDetailViewModel : PageViewModelBase
{
    public NotificationDetailViewModel(
        ServiceProvider services,
        NotificationViewModel notification)
        : base(services)
    {
        Title = notification.Title;
        Description = notification.Description;

        BackCommand = ReactiveCommand.Create(() => { GoBack(); });
    }

    public string Title { get; }
    public string Description { get; }
    public ReactiveCommand<Unit, Unit> BackCommand { get; }

    private void GoBack()
    {
        Services.GetService<NavigationService>()?
            .NavigateTo(new NotificationPageViewModel(Services));
    }
}