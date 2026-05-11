using System;
using System.Reactive;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Notifications;

public class NotificationDetailViewModel : PageViewModelBase
{
    public string Title { get; }
    public string Description { get; }

    public ReactiveCommand<Unit, Unit> ApproveCommand { get; }
    public ReactiveCommand<Unit, Unit> RejectCommand { get; }

    public NotificationDetailViewModel(
        ServiceProvider services,
        NotificationViewModel notification)
        : base(services)
    {
        Title = notification.Title;
        Description = notification.Description;

        ApproveCommand = ReactiveCommand.Create(() =>
        {
            ApproveRegistration();
        });

        RejectCommand = ReactiveCommand.Create(() =>
        {
            RejectRegistration();
        });
    }

    private void ApproveRegistration()
    {
        Console.WriteLine("Registratie goedgekeurd");
        // als de registratie wordt gemaakt kunnen we de consolewriteline voor:
        // await _backend.Registrations.Approve(registrationId);
        
        Services.GetService<NavigationService>()?
            .NavigateTo(new NotificationPageViewModel(Services));
    }

    private void RejectRegistration()
    {
        Console.WriteLine("Registratie afgewezen");

        // als de registratie wordt gemaakt kunnen we de consolewriteline voor:
        // await _backend.Registrations.Reject(registrationId);
        Services.GetService<NavigationService>()?
            .NavigateTo(new NotificationPageViewModel(Services));
    }
}