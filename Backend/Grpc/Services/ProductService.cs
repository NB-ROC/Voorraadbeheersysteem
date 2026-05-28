using Backend.Database.Managers;
using Backend.Entities;
using Backend.Grpc.Helpers;
using Backend.Grpc.Validation;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Protos.Product;
using Category = Protos.Product.Category;
using Role = Protos.Product.Role;

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

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<ProductPageResponse> Page(ProductPageRequest request, ServerCallContext context)
    {
        _validator.ValidatePage(request);

        List<Product> products = await _manager.Page(request.Page, request.PageSize);

        IEnumerable<MetaProduct> metaProducts = products.Select(MapMeta);
        return new ProductPageResponse
        {
            Products = { metaProducts }
        };
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<ProductGetResponse> Get(ProductGetRequest request, ServerCallContext context)
    {
        _validator.ValidateGet(request);

        Product? product = await _manager.Get(request.Id);

        if (product == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Invalid product"));

        return new ProductGetResponse
        {
            Product = MapMeta(product)
        };
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
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
            Description = request.Description,
            Category = new Entities.Category
            {
                Id = request.Category.Id,
                Name = request.Category.Name
            },
            RoleId = request.RoleId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Image = imageName
        };

        return new ProductCreateResponse
        {
            Success = await _manager.Create(product)
        };
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<ProductModifyResponse> Modify(ProductModifyRequest request, ServerCallContext context)
    {
        (Product product, string extension) = await _validator.ValidateModify(request);

        if (request.HasName) product.Name = request.Name;
        if (request.HasDescription) product.Description = request.Description;
        if (request.Category != null)
            product.Category = new Entities.Category
            {
                Id = request.Category.Id,
                Name = request.Category.Name
            };
        if (request.HasRoleId) product.RoleId = request.RoleId;

        product.UpdatedAt = DateTime.UtcNow;

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

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<ProductDeleteResponse> Delete(ProductDeleteRequest request, ServerCallContext context)
    {
        string image = await _validator.ValidateDelete(request);

        StorageHelper.DeleteFile(Path.Combine(ImagePath, image));

        return new ProductDeleteResponse
        {
            Success = await _manager.Delete(request.Id)
        };
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
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

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<ProductRoleResponse> Role(ProductRoleRequest request, ServerCallContext context)
    {
        ProductRoleResponse response = new();
        response.Roles.AddRange((await _manager.Role()).Select(r => new Role
        {
            Id = (int)r.Id,
            Name = r.Name
        }));
        return response;
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<ProductCategoryResponse> Category(ProductCategoryRequest request,
        ServerCallContext context)
    {
        ProductCategoryResponse response = new();
        response.Categories.AddRange((await _manager.Category()).Select(c => new Category
        {
            Id = c.Id,
            Name = c.Name
        }));
        return response;
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<ProductLenderRoleResponse> LenderRole(ProductLenderRoleRequest request,
        ServerCallContext context)
    {
        ProductLenderRoleResponse response = new();
        response.Roles.AddRange((await _manager.Role()).Select(r => new Role
        {
            Id = (int)r.Id,
            Name = r.Name
        }));
        return response;
    }

    private static MetaProduct MapMeta(Product product)
    {
        return new MetaProduct
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = new Category
            {
                Id = product.CategoryId,
                Name = product.Category.Name
            },
            Image = product.Image
        };
    }
}