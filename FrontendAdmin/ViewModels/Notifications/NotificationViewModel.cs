using System.Reactive;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Notifications;

public class NotificationViewModel : ViewModelBase
{
    private readonly NotificationModel _model;

    public NotificationViewModel(INavigationService navigation, NotificationModel model)
    {
        _model = model;

        ViewCommand = ReactiveCommand.Create(() =>
        {
            // TODO: Go to detail page
        });
    }

    public string Title => _model.Title;

    public string Description => _model.Description;

    public ReactiveCommand<Unit, Unit> ViewCommand { get; }
}