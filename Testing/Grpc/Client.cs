using Grpc.Net.Client;
using Shared;

namespace Testing.Grpc;

public static class Client
{
    private const string GrpcChannelIp = "http://127.0.0.1:8080";

    private static readonly GrpcChannel Channel = GrpcChannel.ForAddress(GrpcChannelIp);

    public static readonly Users.UsersClient Users = new(Channel);
}