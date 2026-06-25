using Backend.Database.Managers;
using Backend.Entities;
using Backend.Entities.Relations;
using Grpc.Core;
using Protos.Loan;

namespace Backend.Grpc.Services;

public class LoanService : Loans.LoansBase
{
    private readonly LoanManager _manager;

    public LoanService(LoanManager manager)
    {
        _manager = manager;
    }
    
    public override async Task<LoanPageResponse> Page(LoanPageRequest request, ServerCallContext context)
    {
        List<Loan> loans = await _manager.Page(request.Page, request.PageSize);

        LoanPageResponse response = new LoanPageResponse();
        response.Loans.AddRange(loans.Select(MapMeta));
        return response;
    }

    public override async Task<LoanGetResponse> Get(LoanGetRequest request, ServerCallContext context)
    {
        Loan? loan = await _manager.Get(request.Id);

        return new LoanGetResponse
        {
            Loan = loan != null ? MapMeta(loan) : null
        };
    }

    public override async Task<LoanCreateResponse> Create(LoanCreateRequest request, ServerCallContext context)
    {
        Loan loan = new Loan
        {
            UserId = request.UserId,
            LenderId = request.LenderId,
            DueAt = new DateTime(request.DueAt)
        };

        List<LoanProduct> loanProducts = request.Products.Select(lp => new LoanProduct
        {
            Amount = lp.Amount,
            ProductId = lp.ProductId
        }).ToList();

        return new LoanCreateResponse
        {
            Success = await _manager.Create(loan, loanProducts)
        };
    }

    public override async Task<LoanModifyResponse> Modify(LoanModifyRequest request, ServerCallContext context)
    {
        Loan? loan = await _manager.Get(request.Id);
        if (loan == null) return new LoanModifyResponse { Success = false };
        return new LoanModifyResponse
        {
            Success = await _manager.Modify(loan, request.Products.Select(mlp => new LoanProduct
            {
                Amount = mlp.Amount,
                LoanId =  loan.Id,
                ProductId = mlp.ProductId
            }).ToList())
        };
    }

    private static MetaLoan MapMeta(Loan loan)
    {
        MetaLoan meta = new()
        {
            Id = loan.Id,
            User = UserService.MapMeta(loan.User),
            Lender = UserService.MapMeta(loan.Lender),
            LoanedAt = loan.LoanedAt.Ticks,
            DueAt = loan.DueAt.Ticks,
            ReturnedAt = loan.LoanedAt.Ticks
        };
        meta.Products.AddRange(
            loan.Products.Select(MapMetaLoanProduct)
        );

        return meta;
    }

    private static MetaLoanProduct MapMetaLoanProduct(LoanProduct loanProduct)
    {
        return new MetaLoanProduct
        {
            Amount = loanProduct.Amount,
            Returned = loanProduct.Returned,
            Product = ProductService.MapMeta(loanProduct.Product)
        };
    }
}