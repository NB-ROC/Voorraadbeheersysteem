using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FrontendAdmin.Models;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace FrontendAdmin.ViewModels.Loan;

public class LoanPageViewModel : PageViewModelBase
{
    private readonly ObservableCollection<LoanViewModel> _allLoans =
    [
        new LoanViewModel(new LoanModel
        {
            ProductName = "Laptop",
            BorrowerNumber = "123456",
            LoanDate = "01-04-2026",
            ReturnDate = "15-04-2026",
            Status = "Active",
            Image = "avares://FrontendAdmin/Assets/laptop.png"
        }),

        new LoanViewModel(new LoanModel
        {
            ProductName = "Book",
            BorrowerNumber = "6767676",
            LoanDate = "20-03-2026",
            ReturnDate = "25-03-2026",
            Status = "Overdue",
            Image = "avares://FrontendAdmin/Assets/boek.jpg"
        }),

        new LoanViewModel(new LoanModel
        {
            ProductName = "Tablet",
            BorrowerNumber = "345678",
            LoanDate = "01-02-2026",
            ReturnDate = "10-02-2026",
            Status = "Returned",
            Image = "avares://FrontendAdmin/Assets/tablet.png"
        })

    ];

    public ObservableCollection<LoanViewModel> FilteredLoans { get; } = new();

    private string _productQuery = "";
    public string ProductQuery
    {
        get => _productQuery;
        set => this.RaiseAndSetIfChanged(ref _productQuery, value);
    }

    private string _borrowerQuery = "";
    public string BorrowerQuery
    {
        get => _borrowerQuery;
        set => this.RaiseAndSetIfChanged(ref _borrowerQuery, value);
    }

    public ReactiveCommand<string, Unit> FilterStatusCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    public LoanPageViewModel(ServiceProvider services) : base(services)
    {

        this.WhenAnyValue(x => x.ProductQuery, x => x.BorrowerQuery)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => ApplyFilters());

        FilterStatusCommand = ReactiveCommand.Create<string>(FilterByStatus);
        ResetCommand = ReactiveCommand.Create(ResetFilters);

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var productQuery  = ProductQuery.ToLower();
        var borrowerQuery = BorrowerQuery.ToLower();

        var filtered = _allLoans.Where(l =>
            (l.ProductName.ToLower().Contains(productQuery) ||
             l.LoanDate.ToLower().Contains(productQuery)) &&
            l.BorrowerNumber.ToLower().Contains(borrowerQuery));

        UpdateFilteredLoans(filtered);
    }

    private void FilterByStatus(string status)
    {
        var productQuery  = ProductQuery.ToLower();
        var borrowerQuery = BorrowerQuery.ToLower();

        var filtered = _allLoans.Where(l =>
            l.Status == status &&
            (l.ProductName.ToLower().Contains(productQuery) ||
             l.LoanDate.ToLower().Contains(productQuery)) &&
            l.BorrowerNumber.ToLower().Contains(borrowerQuery));

        UpdateFilteredLoans(filtered);
    }

    private void ResetFilters()
    {
        ProductQuery  = "";
        BorrowerQuery = "";

        ApplyFilters();
    }

    private void UpdateFilteredLoans(IEnumerable<LoanViewModel> loans)
    {
        FilteredLoans.Clear();

        foreach (var loan in loans)
            FilteredLoans.Add(loan);
    }
}