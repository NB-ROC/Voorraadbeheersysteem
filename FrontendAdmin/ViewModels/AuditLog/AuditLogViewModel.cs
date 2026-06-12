using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using FrontendAdmin.Models;
using FrontendAdmin.Services;
using FrontendAdmin.ViewModels.Loan;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.AuditLog;
 
public class AuditLogViewModel : ViewModelBase
{
    private readonly AuditLogModel _model;

    public AuditLogViewModel(
        ServiceProvider services,
        AuditLogModel model)
        : base(services)
    {
        _model = model;
    }
    public int Id => _model.Id;
 
    public string Timestamp => _model.Timestamp;
 
    public string ActorName => _model.ActorName;
 
    public string Action => _model.Action;
 
    public string EntityType => _model.EntityType;
 
    public string EntityId => _model.EntityId;
 
    public string Description => _model.Description;
 
    public string Title =>
        $"{ActorName} - {Action}";

    public string Subtitle =>
        $"{EntityType} ({EntityId})";
 
    //kleurcode per actie voor de badge in de View
    public string ActionColor => Action switch
    {
        "CREATE" => "#16A34A",
        "UPDATE" => "#D97706",
        "DELETE" => "#DC2626",
        "LOGIN"  => "#7C3AED",
        "LOAN"   => "#2563EB",
        _        => "#6B7280"
    };
}