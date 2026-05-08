using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using FrontendAdmin.Models;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Protos.Auth;
using Protos.Product;
using Protos.User;

namespace FrontendAdmin.Services;

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
        Users = new UserEndpoint(new Users.UsersClient(invoker));
    }

    private string Token { get; set; } = string.Empty;

    public Auth.AuthClient AuthClient { get; }

    public UserEndpoint Users { get; }
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

public class UserEndpoint
{
    private readonly Users.UsersClient _client;

    public UserEndpoint(Users.UsersClient client)
    {
        _client = client;
    }

    public async Task<(RequestResult, List<UserModel>)> Page(int page, int pageSize)
    {
        UserPageRequest request = new()
        {
            Page = page,
            PageSize = pageSize
        };

        UserPageResponse? response;
        try
        {
            response = await _client.PageAsync(request);
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), null!);
        }

        return
        (
            RequestResult.Success,
            response.Users.Select(MapUser).ToList()
        );
    }

    public async Task<(RequestResult, UserModel?)> Get(int id)
    {
        UserGetRequest request = new()
        {
            Id = id
        };

        UserGetResponse? response;
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
            MapUser(response.User)
        );
    }

    public async Task<(RequestResult, bool)> Create(UserModel user)
    {
        UserCreateRequest request = new()
        {
            CardId = ByteString.CopyFrom(user.CardId),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Number = user.Number
        };

        UserCreateResponse? response;
        try
        {
            response = await _client.CreateAsync(request);
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), false);
        }

        return (RequestResult.Success, response.Success);
    }

    public async Task<(RequestResult, bool)> Modify(UserModel user)
    {
        UserModifyRequest request = new()
        {
            Id = user.Id,
            CardId = ByteString.CopyFrom(user.CardId),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Number = user.Number
        };

        UserModifyResponse? response;
        try
        {
            response = await _client.ModifyAsync(request);
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), false);
        }

        return (RequestResult.Success, response.Success);
    }

    public async Task<(RequestResult, bool)> Delete(int id)
    {
        UserDeleteRequest request = new()
        {
            Id = id
        };

        UserDeleteResponse? response;
        try
        {
            response = await _client.DeleteAsync(request);
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), false);
        }

        return (RequestResult.Success, response.Success);
    }

    private static UserModel MapUser(MetaUser user)
    {
        return new UserModel
        {
            Id = user.Id,
            CardId = user.CardId.ToByteArray(),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Number = user.Number
        };
    }

    private static RequestResult GetFailCode(RpcException e)
    {
        return e.StatusCode == StatusCode.PermissionDenied ? RequestResult.Denied : RequestResult.Failed;
    }
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

    public async Task<(RequestResult, bool)> Create(ProductModel productModel, byte[]? imageBytes)
    {
        ProductCreateRequest request = new()
        {
            Name = productModel.Name,
            Description = productModel.Description,
            CategoryId = productModel.CategoryModel.Id,
            RoleId = productModel.RoleModel.Id,
            Image = ByteString.CopyFrom(imageBytes)
        };

        ProductCreateResponse? response;
        try
        {
            response = await _client.CreateAsync(request);
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), false);
        }

        return (RequestResult.Success, response.Success);
    }

    public async Task<(RequestResult, bool)> Modify(ProductModel productModel, byte[]? imageBytes)
    {
        ProductModifyRequest request = new()
        {
            Id = productModel.Id,
            Name = productModel.Name,
            Description = productModel.Description,
            CategoryId = productModel.CategoryModel.Id,
            RoleId = productModel.RoleModel.Id,
            Image = ByteString.CopyFrom(imageBytes)
        };

        if (imageBytes != null)
            request.Image = ByteString.CopyFrom(imageBytes);

        ProductModifyResponse? response;
        try
        {
            response = await _client.ModifyAsync(request);
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), false);
        }

        return (RequestResult.Success, response.Success);
    }

    public async Task<(RequestResult, bool)> Delete(int id)
    {
        ProductDeleteRequest request = new()
        {
            Id = id
        };

        ProductDeleteResponse? response;
        try
        {
            response = await _client.DeleteAsync(request);
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), false);
        }

        return (RequestResult.Success, response.Success);
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
    
    public async Task<(RequestResult, List<RoleModel>)> Role()
    {
        ProductRoleResponse? response;
        try
        {
            response = await _client.RoleAsync(new ProductRoleRequest());
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), []);
        }

        return (RequestResult.Success, response.Roles.Select(c => new RoleModel
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
            CategoryModel = new CategoryModel
            {
                Id = product.Category.Id,
                Name = product.Category.Name
            },
            RoleModel = new RoleModel
            {
                Id = product.Id,
                Name = product.Name
            },
            ImageName = product.Image
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