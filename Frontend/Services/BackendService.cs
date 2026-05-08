using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Frontend.Models;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Protos.Auth;
using Protos.Product;
using Protos.User;

namespace Frontend.Services;

public class BackendService
{
    private const string GrpcChannelIp = "http://127.0.0.1:8080";

    public BackendService()
    {
        GrpcChannel channel = GrpcChannel.ForAddress(GrpcChannelIp);
        TokenInjector injector = new(() => Token);

        CallInvoker invoker = channel.Intercept(injector);

        AuthClient = new Auth.AuthClient(channel);
        Products = new ProductEndpoint(new Products.ProductsClient(invoker));
    }

    private string Token { get; set; } = string.Empty;

    public Auth.AuthClient AuthClient { get; }

    public ProductEndpoint Products { get; }

    public async Task<bool> LogIn(string email, string password)
    {
        AuthLoginResponse? response;
        try
        {
            response = await AuthClient.LoginAsync(new AuthLoginRequest
            {
                Email = email,
                Password = password
            });
        }
        catch (RpcException e)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(response.Token))
            return false;

        Token = response.Token;
        return true;
    }
}

#region Interceptors

internal class TokenInjector : Interceptor
{
    private readonly Func<string?> _getToken;

    public TokenInjector(Func<string?> getToken)
    {
        _getToken = getToken;
    }

    private ClientInterceptorContext<TRequest, TResponse> AddToken<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context)
        where TRequest : class
        where TResponse : class
    {
        Metadata headers = context.Options.Headers ?? new Metadata();

        string? token = _getToken();

        if (!string.IsNullOrWhiteSpace(token)) headers.Add("Authorization", $"Bearer {token}");

        CallOptions options = context.Options.WithHeaders(headers);

        return new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            options
        );
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        return continuation(request, AddToken(context));
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        return continuation(request, AddToken(context));
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        return continuation(request, AddToken(context));
    }

    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        return continuation(AddToken(context));
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        return continuation(AddToken(context));
    }
}

#endregion

#region Endpoints

public enum RequestResult
{
    Success,
    Failed,
    Denied
}

public class ProductEndpoint
{
    private readonly Products.ProductsClient _client;

    public ProductEndpoint(Products.ProductsClient client)
    {
        _client = client;
    }

    public async Task<(RequestResult, List<ProductModel>)> Page(int page, int pageSize)
    {
        ProductPageRequest request = new()
        {
            Page = page,
            PageSize = pageSize
        };

        ProductPageResponse? response;
        try
        {
            response = await _client.PageAsync(request);
        }
        catch (RpcException e)
        {
            Console.WriteLine(e.Message);
            return (GetFailCode(e), null!);
        }

        return
        (
            RequestResult.Success,
            response.Products.Select(MapProduct).ToList()
        );
    }

    public async Task<(RequestResult, ProductModel?)> Get(int id)
    {
        ProductGetRequest request = new()
        {
            Id = id
        };

        ProductGetResponse? response;
        try
        {
            response = await _client.GetAsync(request);
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), null);
        }

        return
        (
            RequestResult.Success,
            MapProduct(response.Product)
        );
    }

    public async Task<(RequestResult, (byte[] bytes, Bitmap bitmap)?)> Image(string name)
    {
        ProductImageRequest request = new()
        {
            Name = name
        };

        try
        {
            using AsyncServerStreamingCall<ProductImageResponse>? call = _client.Image(request);

            await foreach (ProductImageResponse response in call.ResponseStream.ReadAllAsync())
            {
                byte[]? bytes = response.Raw.ToByteArray();

                using MemoryStream stream = new(bytes);

                return
                (
                    RequestResult.Success,
                    (stream.ToArray(), new Bitmap(stream))
                );
            }

            return (RequestResult.Failed, null);
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), null);
        }
    }

    public async Task<(RequestResult, List<CategoryModel>)> Category()
    {
        ProductCategoryResponse? response;
        try
        {
            response = await _client.CategoryAsync(new ProductCategoryRequest());
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), []);
        }

        return (RequestResult.Success, response.Categories.Select(c => new CategoryModel
        {
            Id  = c.Id,
            Name = c.Name
        }).ToList());
    }
    private static ProductModel MapProduct(MetaProduct product)
    {
        return new ProductModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = new CategoryModel
            {
                Id = product.Category.Id,
                Name = product.Category.Name
            },
            Image = product.Image
        };
    }

    private static RequestResult GetFailCode(RpcException e)
    {
        return e.StatusCode == StatusCode.PermissionDenied
            ? RequestResult.Denied
            : RequestResult.Failed;
    }
}

#endregion