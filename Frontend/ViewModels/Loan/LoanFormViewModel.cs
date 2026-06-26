using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Threading;
using ExCSS;
using Frontend.Models;
using Frontend.Services;
using Frontend.ViewModels.Components;
using ReactiveUI;

namespace Frontend.ViewModels.Loan;

public class LoanFormViewModel : FormViewModelBase<LoanModel>, IDataErrorInfo
{
    private readonly IApiService _api;
    private readonly ISmartCardService _smartCard;
    private readonly INavigationService _navigation;
    
    public LoanFormViewModel(HeaderViewModel header, FooterViewModel footer, IApiService api, ISmartCardService smartCard, INavigationService navigation) : base(header, footer)
    {
        _api = api;
        _smartCard = smartCard;
        _navigation = navigation;

        ResetUserIdCommand = ReactiveCommand.Create(ResetUserId);
        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
    }

    #region Properties

    public int? Id { get; private set; }
    public int? UserId
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(ResetCardVisible));
        }
    }
    public List<ProductModel> Products { get; set; } = [];

    public DateTimeOffset DueDate
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = DateTime.Now + TimeSpan.FromDays(14);

    public TimeSpan DueTime
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = TimeSpan.Zero;
    
    public DateTime DueAt => DueDate.Date + DueTime;

    #endregion
    
    #region Scanner

    public string UserIdText
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(ResetCardVisible));
        }
    }

    private bool OnScan(byte[] uid)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            (RequestResult requestResult, (int id, string email, string name)? tuple) = await _api.Users.LenderScan(uid);
            if (requestResult == RequestResult.Success && tuple != null)
            {
                UserId = tuple.Value.id;
                UserIdText = "Gebruikersinformatie opgehaald!";
            }
            else
            {
                UserIdText = "Gebruiker niet gevonden, probeer het opnieuw..";
                _smartCard.SetCardDetectedCallback(OnScan);
            }
        });
        return true;
    }

    private void ResetUserId()
    {
        UserId = null;
        UserIdText = "Wachten op scanner-invoer...";
        _smartCard.SetCardDetectedCallback(OnScan);
    }

    public ICommand ResetUserIdCommand { get; }

    #endregion

    #region UI

    public ObservableCollection<RoleSelectionViewModel> Roles { get; } = [];
    
    public bool ResetCardVisible => UserId != null;

    #endregion

    #region Validation
    
        public string this[string columnName]
        {
            get
            {
                ValidationContext context = new(this) { MemberName = columnName };
                List<ValidationResult> results = [];
                object? value = GetType().GetProperty(columnName)?.GetValue(this);
                Console.WriteLine(columnName);
    
                if (!Validator.TryValidateProperty(value, context, results))
                {
                    string? message = results.First().ErrorMessage;
                    if (!string.IsNullOrEmpty(message)) return message;
                }
    
                return string.Empty;
            }
        }
    
        public string Error
        {
            get;
            private set => this.RaiseAndSetIfChanged(ref field, value);
        } = string.Empty;

        private bool IsFormValid =>
            UserId != null &&
            DueAt > DateTime.Now &&
            Products.Count != 0;
        
        #endregion

        #region Saving

        public ICommand SaveCommand { get; }

        private async Task SaveAsync()
        {
            if (!IsFormValid) return;
            
            LoanModel loan = new()
            {
                UserId = UserId?? throw new NullReferenceException("User id null while saving"),
                DueAt = DueAt,
                Products = Products.Select(pm => new LoanProductModel
                {
                    ProductId =  pm.Id,
                    Returned = false,
                    Amount = pm.Amount
                }).ToList()
            };
            
            (RequestResult result, bool success) = await _api.Loans.Create(loan);

            if (result != RequestResult.Success)
            {
                Error = "Er ging iets mis tijdens het opslaan";
                return;
            }

            await _navigation.NavigateTo<LoanPageViewModel>();
        }

        #endregion
    

    #region Loading
    
    public async override Task LoadAsync(LoanModel? existing)
    {
        if (existing != null) throw new NotImplementedException(); // TODO: Make this useful for product hand ins
        
        ResetFields();
    }

    private void ResetFields()
    {
        Id = null;
        Products.Clear();
        DueTime = TimeSpan.Zero;
        DueDate = DateTimeOffset.Now;
        ResetUserId();
    }

    #endregion

}