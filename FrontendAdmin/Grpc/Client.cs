using Grpc.Net.Client;
using Protos.Product;
using Protos.User;

namespace FrontendAdmin.Grpc;

public static class Client
{
    private const string GrpcChannelIp = "http://127.0.0.1:8080";

    private static readonly GrpcChannel Channel = GrpcChannel.ForAddress(GrpcChannelIp);

    public static readonly Products.ProductsClient Products = new(Channel);
    public static readonly Users.UsersClient Users = new(Channel);
}