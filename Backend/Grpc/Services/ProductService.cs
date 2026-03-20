using Backend.Database.Managers;
using Backend.Entities;
using Backend.Grpc.Validation;
using Grpc.Core;
using Shared;

namespace Backend.Grpc.Services;

public class ProductService : Products.ProductsBase
{
    private readonly ProductManager _manager;
    private readonly ProductValidator _validator;

    public ProductService(ProductManager manager)
    {
        _manager = manager;
        _validator = new ProductValidator(manager);
    }

    public override async Task<ProductPageResponse> Page(ProductPageRequest request, ServerCallContext context)
    {
        _validator.ValidatePage(request);
        List<Product> products = await _manager.Page(request.Page, request.PageSize);

        ProductPageResponse response = new();

        response.Products.AddRange(products.Select(product => new MetaProduct
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Description = product.Description,
            Amount = product.Amount,
            Image = product.Image
        }));

        return response;
    }

    public override async Task<ProductGetResponse> Get(ProductGetRequest request, ServerCallContext context)
    {
        _validator.ValidateGet(request);
        Product? product = await _manager.Get(request.Id);

        if (product == null) throw new RpcException(new Status(StatusCode.NotFound, "Invalid product"));

        return new ProductGetResponse
        {
            Product = new MetaProduct
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Description = product.Description,
                Amount = product.Amount,
                Image = product.Image
            }
        };
    }

    public override async Task<ProductCreateResponse> Create(IAsyncStreamReader<ProductCreateRequest> stream, ServerCallContext context)
    {
        
        var request = new ProductCreateRequest();
        await foreach (var message in stream.ReadAllAsync())
        {
            if (message.Name != null) request.Name = message.Name;
            if (message.Category != null) request.Category = message.Category;
            if (message.Description != null) request.Description = message.Description;
            if (message.Amount != null) request.Amount = message.Amount;
            if (message.Image != null) request.Image = message.Image;
        }
        
        string extension = _validator.ValidateCreate(request);
        return new ProductCreateResponse
        {
            Success = await _manager.Create(new Product
            {
                Name = request.Name,
                Category = request.Category,
                Description = request.Description,
                Amount = request.Amount!.Value,
            })
        };
    }
}