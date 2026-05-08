using Google.Protobuf;
using Grpc.Core;
using Protos.Auth;
using Protos.Product;
using Protos.User;
using Testing.Grpc;

namespace Testing;

public class ServiceTests
{
    private string Token;

    [SetUp]
    public void Setup()
    {
        Token = Client.Auth.Login(new AuthLoginRequest
        {
            Email = "testmail@roc-nijmegen.nl",
            Password = "Placeholder1"
        }).Token;
    }

    [Test]
    public void UserTest()
    {
        byte[] cardId = [12, 12, 12, 12, 12, 12, 12];

        List<MetaUser> sizeOne = Client.Users.Page(new UserPageRequest
        {
            Page = 1,
            PageSize = 10
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).Users.ToList();

        bool create = Client.Users.Create(new UserCreateRequest
        {
            CardId = ByteString.CopyFrom(cardId),
            Email = "1234567@student.roc-nijmegen.nl",
            FirstName = "Regu",
            LastName = "Larjoe",
            Number = 123456,
            IsBlocked = true
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).Success;

        List<MetaUser> sizeTwo = Client.Users.Page(new UserPageRequest
        {
            Page = 1,
            PageSize = 10
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).Users.ToList();

        MetaUser createdUser = Client.Users.Get(new UserGetRequest
        {
            Id = 2
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).User;

        bool modify = Client.Users.Modify(new UserModifyRequest
        {
            Id = 2,
            FirstName = "Cheese",
            LastName = "Master"
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).Success;

        MetaUser modifiedUser = Client.Users.Get(new UserGetRequest
        {
            Id = 2
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).User;

        bool delete = Client.Users.Delete(new UserDeleteRequest
        {
            Id = 2
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).Success;

        List<MetaUser> sizeTwoAfterDelete = Client.Users.Page(new UserPageRequest
        {
            Page = 1,
            PageSize = 10
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).Users.ToList();


        Assert.IsNotEmpty(sizeOne);
        Assert.IsTrue(create);
        Assert.AreEqual(sizeTwo.Count, 2);
        Assert.AreEqual(createdUser.Id, 2);
        Assert.IsTrue(modify);
        Assert.AreNotEqual(createdUser, modifiedUser);
        Assert.IsTrue(delete);
        Assert.IsNotEmpty(sizeTwoAfterDelete);

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
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).Products.ToList();

        byte[] localImageBytes =
            await File.ReadAllBytesAsync(Path.Combine(Directory.GetCurrentDirectory(), "Assets/borger.jpg"));
        ProductCreateRequest createRequest = new()
        {
            Name = "Arduino Uno",
            Description = "Dit is een heel mooi ding met allerlei dingetjes",
            CategoryId = 1,
            Image = ByteString.CopyFrom(localImageBytes)
        };

        bool create = Client.Products.Create(createRequest, [new Metadata.Entry("Authorization", $"Bearer {Token}")])
            .Success;

        List<MetaProduct> size1 = Client.Products.Page(new ProductPageRequest
        {
            Page = 1,
            PageSize = 10
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).Products.ToList();

        MetaProduct createdProduct = Client.Products.Get(new ProductGetRequest
        {
            Id = size1[0].Id
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).Product;

        ProductModifyRequest modifyRequest = new()
        {
            Id = createdProduct.Id,
            Name = "Arduino Dos"
        };

        bool modify = Client.Products.Modify(modifyRequest, [new Metadata.Entry("Authorization", $"Bearer {Token}")])
            .Success;

        MetaProduct modifiedProduct = Client.Products.Get(new ProductGetRequest
        {
            Id = createdProduct.Id
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).Product;


        // --- image call
        using AsyncServerStreamingCall<ProductImageResponse>? call = Client.Products.Image(new ProductImageRequest
        {
            Name = modifiedProduct.Image
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]);

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
        }, [new Metadata.Entry("Authorization", $"Bearer {Token}")]).Products.ToList();

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