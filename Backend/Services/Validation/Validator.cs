using Grpc.Core;

namespace Backend.Services.Validation;

public abstract class Validator
{
    protected static void Throw(string message)
    {
        throw new RpcException(new Status(StatusCode.InvalidArgument, message));
    }
}