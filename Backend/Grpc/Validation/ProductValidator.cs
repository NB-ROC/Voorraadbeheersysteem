using Backend.Database.Managers;
using Backend.Entities;
using Google.Protobuf;
using Protos.Product;

namespace Backend.Grpc.Validation;

public class ProductValidator : Validator
{
    private readonly ProductManager _productManager;

    public ProductValidator(ProductManager productManager)
    {
        _productManager = productManager;
    }

    public void ValidatePage(ProductPageRequest request)
    {
        if (request.Page < 1)
            Throw("Invalid page");

        if (request.PageSize is < 1 or > 100)
            Throw("Invalid page size");
    }

    public void ValidateGet(ProductGetRequest request)
    {
        ValidateId(request.Id);
    }

    public string ValidateCreate(ProductCreateRequest request)
    {
        ValidateName(request.Name);
        ValidateDescription(request.Description);
        // ValidateCategoryId(request.CategoryId);

        return ValidateImage(request.Image);
    }

    public async Task<(Product product, string extension)> ValidateModify(ProductModifyRequest request)
    {
        Product? product = await _productManager.Get(request.Id);

        if (product == null)
            Throw("Invalid product");

        ValidateId(request.Id);

        string extension = "";

        if (request.HasName)
            ValidateName(request.Name);

        if (request.HasDescription)
            ValidateDescription(request.Description);

        // if (request.HasCategory)
        //     ValidateCategoryId(request.Category);

        if (request.HasImage)
            extension = ValidateImage(request.Image);

        return (product!, extension);
    }

    public async Task<string> ValidateDelete(ProductDeleteRequest request)
    {
        ValidateId(request.Id);

        Product? product = await _productManager.Get(request.Id);

        if (product == null)
            Throw("Invalid product");

        return product!.Image;
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            Throw("Invalid id");
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > Product.NameLength)
            Throw("Invalid name");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description) || description.Length > Product.DescriptionLength)
            Throw("Invalid description");
    }

    private static void ValidateCategoryId(int categoryId)
    {
        if (categoryId <= 0)
            Throw("Invalid category");
    }

    private static string ValidateImage(ByteString image)
    {
        string? extension = GetImageFormat(image);

        if (extension == null)
            Throw("Invalid image");

        return extension!;
    }

    private static string? GetImageFormat(ByteString? image)
    {
        if (image == null || image.Length < 4)
            return null;

        // JPG
        if (image[0] == 0xFF &&
            image[1] == 0xD8 &&
            image[2] == 0xFF)
            return "jpg";

        // GIF
        if (image[0] == 0x47 &&
            image[1] == 0x49 &&
            image[2] == 0x46 &&
            image[3] == 0x38)
            return "gif";

        // PNG
        if (image.Length >= 8 &&
            image[0] == 0x89 &&
            image[1] == 0x50 &&
            image[2] == 0x4E &&
            image[3] == 0x47)
            return "png";

        return null;
    }
}