using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FrontendAdmin.Models;
using FrontendAdmin.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FrontendAdmin.ViewModels.Loan;

public class LoanPageViewModel : PageViewModelBase
{
    private readonly ObservableCollection<LoanViewModel> _allLoans =
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

    private string _borrowerQuery = "";

    private string _productQuery = "";

    public LoanPageViewModel(HeaderViewModel header, FooterViewModel footer) : base(header, footer)
    {
        this.WhenAnyValue(x => x.ProductQuery, x => x.BorrowerQuery)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => ApplyFilters());

        FilterStatusCommand = ReactiveCommand.Create<string>(FilterByStatus);
        ResetCommand = ReactiveCommand.Create(ResetFilters);
    }

    public ObservableCollection<LoanViewModel> FilteredLoans { get; } = new();

    public string ProductQuery
    {
        get => _productQuery;
        set => this.RaiseAndSetIfChanged(ref _productQuery, value);
    }

    public string BorrowerQuery
    {
        get => _borrowerQuery;
        set => this.RaiseAndSetIfChanged(ref _borrowerQuery, value);
    }

    public ReactiveCommand<string, Unit> FilterStatusCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    private void ApplyFilters()
    {
        string productQuery = ProductQuery.ToLower();
        string borrowerQuery = BorrowerQuery.ToLower();

        IEnumerable<LoanViewModel> filtered = _allLoans.Where(l =>
            (l.ProductName.ToLower().Contains(productQuery) ||
             l.LoanDate.ToLower().Contains(productQuery)) &&
            l.BorrowerNumber.ToLower().Contains(borrowerQuery));

        UpdateFilteredLoans(filtered);
    }

    private void FilterByStatus(string status)
    {
        string productQuery = ProductQuery.ToLower();
        string borrowerQuery = BorrowerQuery.ToLower();

        IEnumerable<LoanViewModel> filtered = _allLoans.Where(l =>
            l.Status == status &&
            (l.ProductName.ToLower().Contains(productQuery) ||
             l.LoanDate.ToLower().Contains(productQuery)) &&
            l.BorrowerNumber.ToLower().Contains(borrowerQuery));

        UpdateFilteredLoans(filtered);
    }

    private void ResetFilters()
    {
        ProductQuery = "";
        BorrowerQuery = "";

        ApplyFilters();
    }

    private void UpdateFilteredLoans(IEnumerable<LoanViewModel> loans)
    {
        FilteredLoans.Clear();

        foreach (LoanViewModel loan in loans)
            FilteredLoans.Add(loan);
    }

    public override Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}