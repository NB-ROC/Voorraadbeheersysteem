using Google.Protobuf;
using Grpc.Net.Client;
using Shared;

namespace Testing;

public class UserTests
{
    private Users.UsersClient _client;

    [SetUp]
    public void Setup()
    {
        GrpcChannel channel = GrpcChannel.ForAddress("http://127.0.0.1:8080");
        _client = new Users.UsersClient(channel);
    }

    [Test]
    public void GetEmptyTest()
    {
        List<MetaUser>? users = null;
        Assert.DoesNotThrow(() => { users = _client.Get(new GetRequest { Page = 1, PageSize = 10 }).Users.ToList(); });


        Assert.That(users, Is.Empty);
    }

    [Test]
    public void CreateTest()
    {
        MetaUser user = new()
        {
            Id = ByteString.CopyFrom(1, 2, 3, 4, 5, 6, 7),
            Name = "John Doe",
            Email = "3214532@student.roc-nijmegen.nl",
            Number = 3214532,
            Staff = false
        };
    }
}