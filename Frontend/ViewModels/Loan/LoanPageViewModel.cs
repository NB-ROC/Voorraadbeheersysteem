using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Frontend.ViewModels.Loan;

public class LoanPageViewModel : PageViewModelBase
{
    public ObservableCollection<LoanViewModel> AllLoans
    {
        get;
    } = [];

    public LoanPageViewModel(HeaderViewModel header, FooterViewModel footer) : base(header, footer)
    {
        this.WhenAnyValue(x => x.ProductQuery, x => x.BorrowerQuery)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => ApplyFilters());

        FilterStatusCommand = ReactiveCommand.Create<string>(FilterByStatus);
        ResetCommand = ReactiveCommand.Create(ResetFilters);

        ApplyFilters();
    }

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

    private void ApplyFilters()
    {
        string productQuery = ProductQuery.ToLower();

        IEnumerable<LoanViewModel> filtered = AllLoans.Where(l =>
            (l.ProductName.Contains(productQuery, StringComparison.OrdinalIgnoreCase) ||
             l.LoanDate.Contains(productQuery, StringComparison.OrdinalIgnoreCase)) &&
            l.BorrowerNumber.Contains(productQuery, StringComparison.OrdinalIgnoreCase));

        UpdateFilteredLoans(filtered);
    }

    private void FilterByStatus(string status)
    {
        string productQuery = ProductQuery.ToLower();
        string borrowerQuery = BorrowerQuery.ToLower();

        IEnumerable<LoanViewModel> filtered = AllLoans.Where(l =>
            l.Status == status &&
            (l.ProductName.Contains(productQuery, StringComparison.OrdinalIgnoreCase) ||
             l.LoanDate.Contains(productQuery, StringComparison.OrdinalIgnoreCase)) &&
            l.BorrowerNumber.Contains(borrowerQuery, StringComparison.OrdinalIgnoreCase));

        UpdateFilteredLoans(filtered);
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

    public override async Task LoadAsync()
    {
        await LoadLoans();
        ResetFilters();
    }

    private async Task LoadLoans()
    {
        AllLoans.Clear();

        List<LoanViewModel> loans =
        [
            new(new LoanModel
            {
                ProductName = "Laptop",
                BorrowerNumber = "123456",
                LoanDate = "01-04-2026",
                ReturnDate = "15-04-2026",
                Status = "Active",
                Image = "avares://FrontendAdmin/Assets/laptop.png"
            }),

            new(new LoanModel
            {
                ProductName = "Book",
                BorrowerNumber = "6767676",
                LoanDate = "20-03-2026",
                ReturnDate = "25-03-2026",
                Status = "Overdue",
                Image = "avares://FrontendAdmin/Assets/boek.jpg"
            }),

            new(new LoanModel
            {
                ProductName = "Tablet",
                BorrowerNumber = "345678",
                LoanDate = "01-02-2026",
                ReturnDate = "10-02-2026",
                Status = "Returned",
                Image = "avares://FrontendAdmin/Assets/tablet.png"
            })
            
        ];
        
        foreach (LoanViewModel loan in loans)
            AllLoans.Add(loan);
    }
}