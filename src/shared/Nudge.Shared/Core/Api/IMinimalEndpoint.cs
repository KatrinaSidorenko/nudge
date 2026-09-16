using Microsoft.AspNetCore.Routing;

namespace Nudge.Shared.Core.Api;

public interface IMinimalEndpoint
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}
