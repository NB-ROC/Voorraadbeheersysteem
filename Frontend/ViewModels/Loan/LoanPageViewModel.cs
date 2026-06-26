using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Frontend.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.Loan;

public class LoanPageViewModel : PageViewModelBase
{
    private readonly INavigationService _navigation;
    
    public LoanPageViewModel(HeaderViewModel header, FooterViewModel footer, INavigationService navigation) : base(header, footer)
    {
        _navigation = navigation;
        LoanFormCommand = ReactiveCommand.CreateFromTask(async () => await _navigation.NavigateTo<LoanFormViewModel, LoanModel>());
    }

    public ObservableCollection<LoanViewModel> AllLoans
    {
        get;
    } = [];

    public ObservableCollection<LoanViewModel> FilteredLoans { get; } = new();

    public string ProductQuery
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string BorrowerQuery
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public ReactiveCommand<string, Unit> FilterStatusCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }
    public ReactiveCommand<Unit, Unit> LoanFormCommand { get; }

    private void ApplyFilters()
    {
        string productQuery = ProductQuery.ToLower();

        // IEnumerable<LoanViewModel> filtered = AllLoans.Where(l =>
        //     (l.ProductName.Contains(productQuery, StringComparison.OrdinalIgnoreCase) ||
        //      l.LoanDate.Contains(productQuery, StringComparison.OrdinalIgnoreCase)) &&
        //     l.BorrowerNumber.Contains(productQuery, StringComparison.OrdinalIgnoreCase));
        //
        // UpdateFilteredLoans(filtered);
    }

    private void FilterByStatus(string status)
    {
        string productQuery = ProductQuery.ToLower();
        string borrowerQuery = BorrowerQuery.ToLower();

        // IEnumerable<LoanViewModel> filtered = AllLoans.Where(l =>
        //     l.Status == status &&
        //     (l.ProductName.Contains(productQuery, StringComparison.OrdinalIgnoreCase) ||
        //      l.LoanDate.Contains(productQuery, StringComparison.OrdinalIgnoreCase)) &&
        //     l.BorrowerNumber.Contains(borrowerQuery, StringComparison.OrdinalIgnoreCase));
        //
        // UpdateFilteredLoans(filtered);
    }

    private void UpdateFilteredLoans(IEnumerable<LoanViewModel> loans)
    {
        FilteredLoans.Clear();

        foreach (LoanViewModel loan in loans)
            FilteredLoans.Add(loan);
    }

    private void ResetFilters()
    {
        ProductQuery = "";
        BorrowerQuery = "";

        ApplyFilters();
    }

    public override Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}