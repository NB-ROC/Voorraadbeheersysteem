using System;
using System.Reactive;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Notifications;

public class NotificationViewModel : ViewModelBase
{
    private readonly NotificationModel _model;

    public NotificationViewModel(ServiceProvider services, NotificationModel model)
        : base(services)
    {
        _model = model;

        ViewCommand = ReactiveCommand.Create(() =>
        {
            Services.GetService<NavigationService>()?
                .NavigateTo(new NotificationDetailViewModel(Services, this));
        });
    }

    public string Title => _model.Title;

    public string Description => _model.Description;

    public ReactiveCommand<Unit, Unit> ViewCommand { get; }
}