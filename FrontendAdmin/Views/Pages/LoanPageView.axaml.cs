using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FrontendAdmin.Views.Pages;

public partial class LoanPageView : UserControl
{
    private ObservableCollection<LeningItem> _alleLeningen = new();
    private ObservableCollection<LeningItem> _filteredLeningen = new();

    public LoanPageView()
    {
        InitializeComponent();
        
        _alleLeningen = new ObservableCollection<LeningItem>
        {
            new LeningItem 
            { 
                ProductNaam = "Laptop",
                LenerNummer = "123456", 
                UitleendDatum = "01-04-2026", 
                TerugDatum = "15-04-2026", 
                Status = "Actief", 
                StatusKleur = "#2e7d32", 
                Image = LeningItem.LoadFromAssets("avares://FrontendAdmin/Assets/laptop.png")
            },
            new LeningItem 
            { 
                ProductNaam = "Boek", 
                LenerNummer = "234567", 
                UitleendDatum = "20-03-2026", 
                TerugDatum = "25-03-2026", 
                Status = "Te laat", 
                StatusKleur = "#d32f2f", 
                Image = LeningItem.LoadFromAssets("avares://FrontendAdmin/Assets/boek.jpg")
            },
            new LeningItem 
            { 
                ProductNaam = "Tablet", 
                LenerNummer = "345678", 
                UitleendDatum = "01-02-2026", 
                TerugDatum = "10-02-2026", 
                Status = "Ingeleverd", 
                StatusKleur = "#0277bd", 
                Image = LeningItem.LoadFromAssets("avares://FrontendAdmin/Assets/tablet.png")
            },
        };

        _filteredLeningen = new ObservableCollection<LeningItem>(_alleLeningen);
        LeningLijst.ItemsSource = _filteredLeningen;
    }

    private void SearchBox_KeyUp(object sender, KeyEventArgs e)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var productQuery = SearchBox.Text?.ToLower() ?? "";
        var gebruikerQuery = GebruikerSearchBox.Text?.ToLower() ?? "";

        _filteredLeningen.Clear();
        foreach (var item in _alleLeningen.Where(l =>
                     (l.ProductNaam.ToLower().Contains(productQuery) ||
                      l.UitleendDatum.ToLower().Contains(productQuery)) &&
                     l.LenerNummer.ToLower().Contains(gebruikerQuery)))
        {
            _filteredLeningen.Add(item);
        }
    }

    private void FilterButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            var status = btn.Content?.ToString();
            _filteredLeningen.Clear();
            foreach (var item in _alleLeningen.Where(l => l.Status == status))
                _filteredLeningen.Add(item);
        }
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        SearchBox.Text = "";
        GebruikerSearchBox.Text = "";
        _filteredLeningen.Clear();
        foreach (var item in _alleLeningen)
            _filteredLeningen.Add(item);
    }
}

public class LeningItem
{
    public string ProductNaam { get; set; } = "";
    public string LenerNummer { get; set; } = "";
    public string UitleendDatum { get; set; } = "";
    public string TerugDatum { get; set; } = "";
    public string Status { get; set; } = "";
    public string StatusKleur { get; set; } = "";
    public Bitmap? Image { get; set; }

    public static Bitmap? LoadFromAssets(string uri)
    {
        try
        {
            var stream = AssetLoader.Open(new Uri(uri));
            return new Bitmap(stream);
        }
        catch
        {
            return null;
        }
    }
}