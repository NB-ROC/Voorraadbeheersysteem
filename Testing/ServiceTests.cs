using Google.Protobuf;
using Grpc.Core;
using Protos.Product;
using Protos.User;
using Testing.Grpc;

namespace Testing;

public class ServiceTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void UserTest()
    {
        byte[] userId = [12, 12, 12, 12, 12, 12, 12];

        List<MetaUser> empty = Client.Users.Page(new UserPageRequest
        {
            Page = 1,
            PageSize = 10
        }).Users.ToList();

        bool create = Client.Users.Create(new UserCreateRequest
        {
            Id = ByteString.CopyFrom(userId),
            Email = "1234567@student.roc-nijmegen.nl",
            FirstName = "Regu",
            LastName = "Larjoe",
            Number = 123456,
            IsBlocked = true
        }).Success;

        List<MetaUser> size1 = Client.Users.Page(new UserPageRequest
        {
            Page = 1,
            PageSize = 10
        }).Users.ToList();

        MetaUser createdUser = Client.Users.Get(new UserGetRequest
        {
            Id = ByteString.CopyFrom(userId)
        }).User;

        bool modify = Client.Users.Modify(new UserModifyRequest
        {
            Id = ByteString.CopyFrom(userId),
            FirstName = "Cheese",
            LastName = "Master"
        }).Success;

        MetaUser modifiedUser = Client.Users.Get(new UserGetRequest
        {
            Id = ByteString.CopyFrom(userId)
        }).User;

        bool delete = Client.Users.Delete(new UserDeleteRequest
        {
            Id = ByteString.CopyFrom(userId)
        }).Success;

        List<MetaUser> emptyAfterDelete = Client.Users.Page(new UserPageRequest
            {
                Page = 1,
                PageSize = 10
            })
            .Users.ToList();


        Assert.IsEmpty(empty);
        Assert.IsTrue(create);
        Assert.IsNotEmpty(size1);
        Assert.AreEqual(createdUser.Id.ToByteArray(), userId);
        Assert.IsTrue(modify);
        Assert.AreNotEqual(createdUser, modifiedUser);
        Assert.IsTrue(delete);
        Assert.IsEmpty(emptyAfterDelete);

        Console.WriteLine(createdUser);
        Console.WriteLine(modifiedUser);
    }

    [Test]
    public async Task ProductTest()
    {
        List<MetaProduct> empty = Client.Products.Page(new ProductPageRequest
        {
            Page = 1,
            PageSize = 10
        }).Products.ToList();

        byte[] localImageBytes =
            await File.ReadAllBytesAsync(Path.Combine(Directory.GetCurrentDirectory(), "Assets/borger.jpg"));
        ProductCreateRequest createRequest = new()
        {
            Name = "Arduino Uno",
            Description = "Dit is een heel mooi ding met allerlei dingetjes",
            CategoryId = 1,
            Image = ByteString.CopyFrom(localImageBytes)
        };

        bool create = Client.Products.Create(createRequest).Success;

        List<MetaProduct> size1 = Client.Products.Page(new ProductPageRequest
        {
            Page = 1,
            PageSize = 10
        }).Products.ToList();

        MetaProduct createdProduct = Client.Products.Get(new ProductGetRequest
        {
            Id = size1[0].Id
        }).Product;

        ProductModifyRequest modifyRequest = new()
        {
            Id = createdProduct.Id,
            Name = "Arduino Dos"
        };

        bool modify = Client.Products.Modify(modifyRequest).Success;

        MetaProduct modifiedProduct = Client.Products.Get(new ProductGetRequest
        {
            Id = createdProduct.Id
        }).Product;


        // --- image call
        using AsyncServerStreamingCall<ProductImageResponse>? call = Client.Products.Image(new ProductImageRequest
        {
            Name = modifiedProduct.Image
        });

        using MemoryStream imageStream = new();

        await foreach (ProductImageResponse response in call.ResponseStream.ReadAllAsync())
        {
            byte[] chunk = response.Raw.ToByteArray();
            await imageStream.WriteAsync(chunk, 0, chunk.Length);
        }

        byte[] imageBytes = imageStream.ToArray();
        await File.WriteAllBytesAsync(modifiedProduct.Image, imageBytes);
        // --- image call


        bool delete = Client.Products.Delete(new ProductDeleteRequest
        {
            Id = modifiedProduct.Id
        }).Success;

        List<MetaProduct> emptyAfterDelete = Client.Products.Page(new ProductPageRequest
        {
            Page = 1,
            PageSize = 10
        }).Products.ToList();

        Assert.IsEmpty(empty);
        Assert.IsTrue(create);
        Assert.IsNotEmpty(size1);
        Assert.IsTrue(modify);
        Assert.AreEqual(localImageBytes, imageBytes);
        Assert.AreNotEqual(createdProduct, modifiedProduct);
        Assert.IsTrue(delete);
        Assert.IsEmpty(emptyAfterDelete);

        Console.WriteLine(createdProduct);
        Console.WriteLine(modifiedProduct);
    }
}