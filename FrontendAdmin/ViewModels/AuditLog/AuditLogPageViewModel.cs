using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using FrontendAdmin.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.AuditLog;
 
public class AuditLogPageViewModel : PageViewModelBase
{
    public ObservableCollection<AuditLogViewModel> Logs { get; } = [];
    public ObservableCollection<AuditLogViewModel> FilteredLogs { get; } = [];  
    private int _page = 1;
 
    public AuditLogPageViewModel(ServiceProvider services)
        : base(services)
    {
        this.WhenAnyValue(
                x => x.AuditLogQuery,
                x => x.AdminQuery,
                x => x.SelectedStatus)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => ApplyFilters());
        
        FilterStatusCommand = ReactiveCommand.Create<string>(FilterByStatus);
        ResetCommand = ReactiveCommand.Create(ResetFilters);
        
        ApplyFilters();
        
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
        ApplyFilters();
    }
    private string _adminQuery = "";
    private string _auditLogQuery = "";
    private string _selectedStatus = "";
    
    public string AdminQuery
    {
        get => _adminQuery;
        set => this.RaiseAndSetIfChanged(ref _adminQuery, value);
    }
    public string AuditLogQuery
    {
        get => _auditLogQuery;
        set => this.RaiseAndSetIfChanged(ref _auditLogQuery, value);
    }
    public string SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedStatus, value);
            ApplyFilters();
        }
    }
    public ReactiveCommand<string, Unit> FilterStatusCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }
    private void ApplyFilters()
    {
        string auditLogQuery = (AuditLogQuery ?? "").ToLower();
        string adminQuery = (AdminQuery ?? "").ToLower();

        IEnumerable<AuditLogViewModel> filtered = Logs.Where(l =>
            (
                (l.EntityId?.ToLower().Contains(auditLogQuery) == true ||
                 l.Timestamp?.ToLower().Contains(auditLogQuery) == true ||
                 l.Action?.ToLower().Contains(auditLogQuery) == true ||
                 l.Description?.ToLower().Contains(auditLogQuery) == true)
            )
            &&
            (
                string.IsNullOrWhiteSpace(SelectedStatus) ||
                l.Action?.ToLower() == SelectedStatus
            )
            &&
            (
                l.ActorName?.ToLower().Contains(adminQuery) == true
            )
        );

        UpdateFilteredLogs(filtered);
    }

    private void FilterByStatus(string status)
    {
        SelectedStatus = (status ?? "").ToLowerInvariant();
        ApplyFilters();
    }

    private void ResetFilters()
    {
        AuditLogQuery = "";
        AdminQuery = "";
        
        ApplyFilters();
    }
    
    private void UpdateFilteredLogs(IEnumerable<AuditLogViewModel> logs)
    {
        FilteredLogs.Clear();

        foreach (AuditLogViewModel log in logs)
            FilteredLogs.Add(log);
    }
}