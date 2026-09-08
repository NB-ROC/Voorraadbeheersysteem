using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Frontend.ViewModels.Components;

namespace Frontend.ViewModels.Loan;

public class LoanProductSelectionViewModel : SelectionViewModelBase<ProductModel>
{
    private readonly IApiService _api;
    private Action<ProductModel?> _callback = m => throw new NotImplementedException();
    
    public LoanProductSelectionViewModel(HeaderViewModel header, FooterViewModel footer, IApiService api) : base(header, footer)
    {
        _api = api;
    }

    #region Data

    public ObservableCollection<ProductModel> Data { get; set; } = [];
    
    #endregion

    #region Loading

    public override async Task LoadAsync(Action<ProductModel?> callback, List<ProductModel>? data)
    {
        _callback = callback;
        ResetData();
        await LoadData(data);
    }

    private async Task LoadData(List<ProductModel>? data)
    {
        (RequestResult result, List<ProductModel> products) = await _api.Products.Page(1, 1000);

        if (result == RequestResult.Success)
            foreach (ProductModel product in products)
            {
                Data.Add(product);
            }
    }

    private void ResetData()
    {
        Data.Clear();
    }

    #endregion
    
    
}