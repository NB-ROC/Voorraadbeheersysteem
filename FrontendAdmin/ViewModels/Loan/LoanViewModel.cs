using System;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using FrontendAdmin.Models;

namespace FrontendAdmin.ViewModels.Loan;

public class LoanViewModel
{
    private readonly LoanModel _model;

    public LoanViewModel(LoanModel model)
    {
        _model = model;
    }

    public string ProductName    => _model.ProductName;
    public string BorrowerNumber => _model.BorrowerNumber;
    public string LoanDate       => _model.LoanDate;
    public string ReturnDate     => _model.ReturnDate;
    public string Status         => _model.Status;

    public Bitmap? Image
    {
        get
        {
            if (string.IsNullOrEmpty(_model.Image))
                return null;
            try
            {
                var stream = AssetLoader.Open(new Uri(_model.Image));
                return new Bitmap(stream);
            }
            catch { return null; }
        }
    }

    public IBrush StatusColor => _model.Status switch
    {
        "Active"   => Brushes.Green,
        "Overdue"  => Brushes.Red,
        "Returned" => Brushes.Gray,
        "Pending"  => Brushes.Orange,
        _          => Brushes.Transparent
    };
}