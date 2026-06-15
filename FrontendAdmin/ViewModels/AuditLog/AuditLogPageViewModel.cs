using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Components;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.AuditLog;

public class AuditLogPageViewModel : PageViewModelBase
{
    private readonly IApiService _api;
    private CompositeDisposable _disposables = new();
    private int _page = 1;

    public AuditLogPageViewModel(HeaderViewModel header, FooterViewModel footer, IApiService api) : base(header, footer)
    {
        _api = api;

        FilterStatusCommand = ReactiveCommand.Create<string>(FilterByStatus);
        ResetCommand = ReactiveCommand.Create(ResetFilters);
    }

    public ObservableCollection<AuditLogViewModel> Logs { get; } = [];
    public ObservableCollection<AuditLogViewModel> FilteredLogs { get; } = [];

    #region Filter

    public string AdminQuery
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string AuditLogQuery
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string SelectedStatus
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    private void ApplyFilters()
    {
        string auditLogQuery = AuditLogQuery.ToLower();
        string adminQuery = AdminQuery.ToLower();

        IEnumerable<AuditLogViewModel> filtered = Logs.Where(l =>
            (
                l.EntityId.Contains(auditLogQuery, StringComparison.OrdinalIgnoreCase) ||
                l.Timestamp.Contains(auditLogQuery, StringComparison.OrdinalIgnoreCase) ||
                l.Action.Contains(auditLogQuery, StringComparison.OrdinalIgnoreCase) ||
                l.Description.Contains(auditLogQuery, StringComparison.OrdinalIgnoreCase)
            )
            &&
            (
                string.IsNullOrWhiteSpace(SelectedStatus) ||
                l.Action.Equals(SelectedStatus, StringComparison.OrdinalIgnoreCase)
            )
            &&
            l.ActorName.Contains(adminQuery, StringComparison.OrdinalIgnoreCase)
        );

        UpdateFilteredLogs(filtered);
    }


    public ReactiveCommand<string, Unit> FilterStatusCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    private void FilterByStatus(string status)
    {
        SelectedStatus = (status ?? "").ToLowerInvariant();
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

    #endregion

    #region Loading

    public override async Task LoadAsync()
    {
        await LoadAuditLogs();

        LoadSubscriptions();

        ResetFilters();
        ApplyFilters();
    }

    private async Task LoadAuditLogs()
    {
        (RequestResult result, List<AuditLogModel> logs) = await _api.AuditLogs.Page();

        Logs.Clear();

        if (result != RequestResult.Success || logs.Count == 0) return;

        foreach (AuditLogModel log in logs)
            Logs.Add(new AuditLogViewModel(log));
    }

    private void LoadSubscriptions()
    {
        _disposables.Dispose();
        _disposables = new CompositeDisposable();

        this.WhenAnyValue(
                x => x.AuditLogQuery,
                x => x.AdminQuery,
                x => x.SelectedStatus)
            .Subscribe(_ => ApplyFilters())
            .DisposeWith(_disposables);
    }

    #endregion
}