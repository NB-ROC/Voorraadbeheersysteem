using System.Collections.ObjectModel;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
 
namespace FrontendAdmin.ViewModels.AuditLog;
 
public class AuditLogPageViewModel : PageViewModelBase
{
    public ObservableCollection<AuditLogViewModel> Logs { get; } = [];
 
    private int _page = 1;
 
    public AuditLogPageViewModel(ServiceProvider services)
        : base(services)
    {
        Load();
    }
 
    private async void Load()
    {
        var backend = Services.GetRequiredService<BackendService>();
 
        var (result, logs) = await backend.AuditLogs.Page();
 
        if (result != RequestResult.Success || logs == null)
            return;
 
        Logs.Clear();
 
        foreach (var log in logs)
        {
            Logs.Add(new AuditLogViewModel(Services, log));
        }
    }
}