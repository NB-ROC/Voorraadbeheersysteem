using Google.Protobuf;
using Shared;
using Testing.Grpc;

namespace Testing;

public class UserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CreateUserTest()
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
            Name = "Regu Larjoe",
            Number = 1234567,
            Staff = false
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
        Assert.IsTrue(delete);
        Assert.IsEmpty(emptyAfterDelete);

        Console.WriteLine(createdUser);
    }
}