using System.Security.Claims;
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
    private readonly AuditLogManager _auditLogManager;

    public ProductService(ProductManager manager, AuditLogManager auditLogManager)
    {
        _manager = manager;
        _validator = new ProductValidator(manager);
        _auditLogManager = auditLogManager;
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Image = imageName
        };

        bool success = await _manager.Create(product);
        
        if (request.RoleIds.Count > 0)
        {
            IEnumerable<RoleType> roles = request.RoleIds.Select(id => (RoleType)id);
            await _manager.SetRoles(product.Id, roles);
        }
        if (success)
        {
            await _auditLogManager.Log(
                GetActorId(context),
                "CREATE",
                "Product",
                product.Id.ToString(),
                $"Product '{product.Name}' aangemaakt"
            );
        }
        return new ProductCreateResponse
        {
            Success = success
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

        product.UpdatedAt = DateTime.UtcNow;

        if (request.HasImage)
            product.Image = await StorageHelper.ModifyFile(
                Path.Combine(ImagePath, product.Image),
                request.Image,
                extension,
                ImagePath
            );

        bool success = await _manager.Modify(product);
        
        if (request.RoleIds.Count > 0)
        {
            IEnumerable<RoleType> roles = request.RoleIds.Select(id => (RoleType)id);
            await _manager.SetRoles(product.Id, roles);
        }
        if (success)
        {
            await _auditLogManager.Log(
                GetActorId(context),
                "UPDATE",
                "Product",
                product.Id.ToString(),
                $"Product '{product.Name}' aangepast"
            );
        }
        return new ProductModifyResponse
        {
            Success = success
        };
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)},{nameof(RoleType.Manager)}")]
    public override async Task<ProductDeleteResponse> Delete(
        ProductDeleteRequest request,
        ServerCallContext context)
    {
        string image = await _validator.ValidateDelete(request);

        StorageHelper.DeleteFile(Path.Combine(ImagePath, image));

        bool success = await _manager.Delete(request.Id);

        if (success)
        {
            await _auditLogManager.Log(
                GetActorId(context),
                "DELETE",
                "Product",
                request.Id.ToString(),
                $"Product verwijderd (id {request.Id})"
            );
        }

        return new ProductDeleteResponse
        {
            Success = success
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
    public override async Task<ProductRoleResponse> Role(ProductRoleRequest request, 
        ServerCallContext context)
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
        response.Roles.AddRange((await _manager.Role())
            .Where(role => role.Id > RoleType.Lender)
            .Select(r => new Role
        {
            Id = (int)r.Id,
            Name = r.Name
        }));
        return response;
    }

    public static MetaProduct MapMeta(Product product)
    {
        var meta = new MetaProduct
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
        
        meta.Roles.AddRange(product.ProductRoles.Select(ur => new Role
        {
            Id = (int)ur.Role.Id,
            Name = ur.Role.Name
        }));

        return meta;
    }
    private static int GetActorId(ServerCallContext context)
    {
        return int.Parse(
            context.GetHttpContext().User
                .FindFirst(ClaimTypes.NameIdentifier)!
                .Value);
    }
}