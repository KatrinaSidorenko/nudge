using MediatR;

namespace Nudge.Shared.EFCore;

// ref: https://github.com/meysamhadeli/booking-microservices/blob/main/src/BuildingBlocks/EFCore/EfTxBehavior.cs
//
// Delegates to IUnitOfWork, which saves every registered DbContext's pending changes and
// publishes the domain events raised on their tracked aggregates.
public class EfTxBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        await unitOfWork.SaveChangesAndPublishEventsAsync(cancellationToken);

        return response;
    }
}
