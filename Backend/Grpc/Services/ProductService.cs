using Backend.Database.Managers;
using Backend.Entities;
using Backend.Grpc.Helpers;
using Backend.Grpc.Validation;
using Google.Protobuf;
using Grpc.Core;
using Protos.Product;

namespace Backend.Grpc.Services;

public class ProductService : Products.ProductsBase
{
    private const string ImagePath = "Products/";

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

        response.Products.AddRange(products.Select(MapMeta));

        return response;
    }

    public override async Task<ProductGetResponse> Get(ProductGetRequest request, ServerCallContext context)
    {
        _validator.ValidateGet(request);
        Product? product = await _manager.Get(request.Id);

        if (product == null) throw new RpcException(new Status(StatusCode.NotFound, "Invalid product"));

        return new ProductGetResponse
        {
            Product = MapMeta(product)
        };
    }

    public override async Task<ProductCreateResponse> Create(ProductCreateRequest request, ServerCallContext context)
    {
        string extension = _validator.ValidateCreate(request);

        string imageName = await StorageHelper.SaveFile(
            request.Image,
            extension,
            ImagePath
        );

        Product product = new()
        {
            Name = request.Name,
            Category = request.Category,
            Description = request.Description,
            Amount = request.Amount!.Value,
            Image = imageName
        };

        return new ProductCreateResponse
        {
            Success = await _manager.Create(product)
        };
    }

    public override async Task<ProductModifyResponse> Modify(ProductModifyRequest request, ServerCallContext context)
    {
        (Product product, string extension) = await _validator.ValidateModify(request);

        if (request.HasName) product.Name = request.Name;

        if (request.HasCategory) product.Category = request.Category;

        if (request.HasDescription) product.Description = request.Description;

        if (request.Amount != null) product.Amount = request.Amount.Value;

        if (request.HasImage)
            product.Image = await StorageHelper.ModifyFile(
                Path.Combine(ImagePath, product.Image),
                request.Image,
                extension,
                ImagePath
            );

        return new ProductModifyResponse
        {
            Success = await _manager.Modify(product)
        };
    }

    public override async Task<ProductDeleteResponse> Delete(ProductDeleteRequest request, ServerCallContext context)
    {
        string image = await _validator.ValidateDelete(request);
        StorageHelper.DeleteFile(Path.Combine(ImagePath, image));
        return new ProductDeleteResponse
        {
            Success = await _manager.Delete(request.Id)
        };
    }

    public override async Task Image(ProductImageRequest request,
        IServerStreamWriter<ProductImageResponse> responseStream, ServerCallContext context)
    {
        ByteString file = await StorageHelper.GetFile(Path.Combine(ImagePath, request.Name));

        await responseStream.WriteAsync(new ProductImageResponse
        {
            Raw = file,
            Extension = Path.GetExtension(request.Name).TrimStart('.')
        });
    }

    private static MetaProduct MapMeta(Product product)
    {
        return new MetaProduct
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Description = product.Description,
            Amount = product.Amount,
            Image = product.Image
        };
    }
}