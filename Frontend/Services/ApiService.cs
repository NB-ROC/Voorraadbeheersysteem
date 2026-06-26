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
using Protos.Loan;
using Protos.Product;
using Protos.User;

namespace Frontend.Services;

public interface IApiService
{
    public UserModel? LoggedInUser { get; }
    public UserEndpoint Users { get; }
    public ProductEndpoint Products { get; }
    public LoanEndpoint Loans { get; }

    public Task<bool> LogIn(string email, string password);
}

public class ApiService : IApiService
{
    private const string GrpcChannelIp = "http://127.0.0.1:8080";

    private string Token { get; set; } = string.Empty;

    public UserModel? LoggedInUser { get; private set; }

    private Auth.AuthClient AuthClient { get; }

    public UserEndpoint Users { get; }

    public ProductEndpoint Products { get; }
    
    public LoanEndpoint Loans { get; }

    public ApiService()
    {
        GrpcChannel channel = GrpcChannel.ForAddress(GrpcChannelIp);
        TokenInjector injector = new(() => Token);

        CallInvoker invoker = channel.Intercept(injector);

        AuthClient = new Auth.AuthClient(channel);
        Products = new ProductEndpoint(new Products.ProductsClient(invoker));
        Users = new UserEndpoint(new Users.UsersClient(invoker));
        Loans = new LoanEndpoint(new Loans.LoansClient(invoker));
    }

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
        catch (RpcException)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(response.Token))
            return false;

        Token = response.Token;
        LoggedInUser = new UserModel
        {
            Id = response.User.Id,
            CardId = response.User.CardId.ToByteArray(),
            Email = response.User.Email,
            FirstName = response.User.FirstName,
            LastName = response.User.LastName,
            Number = response.User.Number
        };
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

    public async Task<(RequestResult, bool)> Create(UserModel userModel)
    {
        UserCreateRequest request = new()
        {
            CardId = ByteString.CopyFrom(userModel.CardId),
            Email = userModel.Email,
            FirstName = userModel.FirstName,
            LastName = userModel.LastName,
            Number = userModel.Number
        };

        request.RoleIds.Add(userModel.Roles.Select(r => r.Id));


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

    public async Task<(RequestResult, bool)> Modify(UserModel userModel)
    {
        UserModifyRequest request = new()
        {
            Id = userModel.Id,
            CardId = ByteString.CopyFrom(userModel.CardId),
            FirstName = userModel.FirstName,
            LastName = userModel.LastName,
            Email = userModel.Email,
            Number = userModel.Number
        };

        request.RoleIds.Add(userModel.Roles.Select(r => r.Id));

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

    public async Task<(RequestResult, (int id, string email, string name)?)> LenderScan(byte[] cardId)
    {
        UserLenderScanRequest request = new()
        {
            CardId = ByteString.CopyFrom(cardId)
        };

        UserLenderScanResponse? response;
        try
        {
            response = await _client.LenderScanAsync(request);
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), null);
        }

        return (RequestResult.Success, response.HasEmail ? (response.Id, response.Email, response.Name) : null);
    }

    public async Task<(RequestResult, List<UserModel>)> LenderPage(int page, int pageSize)
    {
        UserLenderPageRequest request = new()
        {
            Page = page,
            PageSize = pageSize
        };

        UserLenderPageResponse? response;
        try
        {
            response = await _client.LenderPageAsync(request);
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


    private static UserModel MapUser(MetaUser user)
    {
        return new UserModel
        {
            Id = user.Id,
            CardId = user.CardId.ToByteArray(),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Number = user.Number,
            Roles = user.Roles.Select(r => new RoleModel
            {
                Id = r.Id,
                Name = r.Name
            }).ToList()
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
            Amount = productModel.Amount,
            Description = productModel.Description,
            Category = new Category
            {
                Id = productModel.CategoryModel.Id,
                Name = productModel.CategoryModel.Name
            },
            Image = ByteString.CopyFrom(imageBytes)
        };

        request.RoleIds.Add(productModel.Roles.Select(r => r.Id));

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
            Amount = productModel.Amount,
            Category = new Category
            {
                Id = productModel.CategoryModel.Id,
                Name = productModel.CategoryModel.Name
            },
            Image = ByteString.CopyFrom(imageBytes)
        };

        request.RoleIds.Add(productModel.Roles.Select(r => r.Id));

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
            Id = c.Id,
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
            Id = c.Id,
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
            Amount = product.Amount,
            CategoryModel = new CategoryModel
            {
                Id = product.Category.Id,
                Name = product.Category.Name
            },
            Roles = product.Roles.Select(r => new RoleModel
            {
                Id = r.Id,
                Name = r.Name
            }).ToList(),
            ImageName = product.Image
        };
    }

    public async Task<(RequestResult, List<RoleModel>)> LenderRole()
    {
        ProductLenderRoleResponse? response;
        try
        {
            response = await _client.LenderRoleAsync(new ProductLenderRoleRequest());
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), []);
        }

        return (RequestResult.Success, response.Roles.Select(c => new RoleModel
        {
            Id = c.Id,
            Name = c.Name
        }).ToList());
    }

    private static RequestResult GetFailCode(RpcException e)
    {
        return e.StatusCode == StatusCode.PermissionDenied
            ? RequestResult.Denied
            : RequestResult.Failed;
    }
}

public class LoanEndpoint
{
    private readonly Loans.LoansClient _client;

    public LoanEndpoint(Loans.LoansClient client)
    {
        _client = client;
    }

    public async Task<(RequestResult, List<LoanModel>)> Page(int page, int pageSize)
    {
        LoanPageRequest request = new()
        {
            Page = page,
            PageSize = pageSize
        };

        LoanPageResponse? response;
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
            response.Loans.Select(MapLoan).ToList()
        );
    }

    public async Task<(RequestResult, LoanModel?)> Get(int id)
    {
        LoanGetRequest request = new()
        {
            Id = id
        };

        LoanGetResponse? response;
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
            response.Loan != null ? MapLoan(response.Loan) : null
        );
    }

    public async Task<(RequestResult, bool)> Create(LoanModel loanModel)
    {
        LoanCreateRequest request = new()
        {
            UserId = loanModel.User.Id,
            DueAt = loanModel.DueAt.ToFileTimeUtc()
        };

        request.Products.Add(loanModel.Products.Select(MapLoanProduct));

        LoanCreateResponse? response;
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

    public async Task<(RequestResult, bool)> Modify(LoanModel loanModel)
    {
        LoanModifyRequest request = new()
        {
            Id = loanModel.Id
        };

        request.Products.Add(loanModel.Products.Select(MapLoanProduct));

        LoanModifyResponse? response;
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
        LoanDeleteRequest request = new()
        {
            Id = id
        };

        LoanDeleteResponse? response;
        try
        {
            response = await _client.DeleteAsync(request);
        }
        catch (RpcException e)
        {
            return (GetFailCode(e), false);
        }

        return (RequestResult.Success, response.Response);
    }

    private static LoanModel MapLoan(MetaLoan loan)
    {
        return new LoanModel
        {
            Id = loan.Id,
            User = new UserModel
            {
                Id = loan.User.Id,
                CardId = loan.User.CardId.ToByteArray(),
                Email = loan.User.Email,
                FirstName = loan.User.FirstName,
                LastName = loan.User.LastName,
                Number = loan.User.Number
            },
            Lender = new UserModel
            {
                Id = loan.Lender.Id,
                CardId = loan.Lender.CardId.ToByteArray(),
                Email = loan.Lender.Email,
                FirstName = loan.Lender.FirstName,
                LastName = loan.Lender.LastName,
                Number = loan.Lender.Number
            },
            LoanedAt = DateTime.FromFileTimeUtc(loan.LoanedAt),
            DueAt = DateTime.FromFileTimeUtc(loan.DueAt),
            ReturnedAt = loan.HasReturnedAt ? DateTime.FromFileTimeUtc(loan.ReturnedAt) : null,
            Products = loan.Products.Select(MapLoanProduct).ToList()
        };
    }

    private static LoanProductModel MapLoanProduct(MetaLoanProduct product)
    {
        return new LoanProductModel
        {
            Amount = product.Amount,
            Returned = product.Returned,
            ProductId = product.ProductId,
            Product = MapProduct(product.Product)
        };
    }

    private static MetaLoanProduct MapLoanProduct(LoanProductModel product)
    {
        return new MetaLoanProduct
        {
            Amount = product.Amount,
            Returned = product.Returned,
            ProductId = product.ProductId
        };
    }

    private static ProductModel MapProduct(MetaProduct product)
    {
        return new ProductModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Amount = product.Amount,
            CategoryModel = new CategoryModel
            {
                Id = product.Category.Id,
                Name = product.Category.Name
            },
            Roles = product.Roles.Select(r => new RoleModel
            {
                Id = r.Id,
                Name = r.Name
            }).ToList(),
            ImageName = product.Image
        };
    }

    private static RequestResult GetFailCode(RpcException e)
    {
        return e.StatusCode == StatusCode.PermissionDenied ? RequestResult.Denied : RequestResult.Failed;
    }
}

#endregion