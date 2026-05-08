using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Protos.Auth;
using Protos.Product;
using Protos.User;

namespace Testing.Grpc;

public static class Client
{
    private const string GrpcChannelIp = "http://127.0.0.1:8080";

    private static readonly GrpcChannel Channel = GrpcChannel.ForAddress(GrpcChannelIp);

    public static readonly Users.UsersClient Users = new(Channel);
    public static readonly Products.ProductsClient Products = new(Channel);
    public static readonly Auth.AuthClient Auth = new(Channel);
}

public class AuthInterceptor : Interceptor
{
    private readonly string _token;

    public AuthInterceptor(string token)
    {
        _token = token;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        Metadata headers = context.Options.Headers ?? new Metadata();
        headers.Add("Authorization", $"Bearer {_token}");

        CallOptions newOptions = context.Options.WithHeaders(headers);
        ClientInterceptorContext<TRequest, TResponse> newContext = new(
            context.Method,
            context.Host,
            newOptions
        );

        return continuation(request, newContext);
    }
}