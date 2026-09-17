using MediatR;
using Nudge.Identity.Users.Models;
using Nudge.Learning.Users.Features.CreateOrActivateDefaultDeck.V1;

namespace Nudge.Learning.Users.EventHandlers;

public class CreateOrActivateDefaultDeckOnUserUpsertedHandler(IMediator mediator) : INotificationHandler<UserUpsertedEvent>
{
    public Task Handle(UserUpsertedEvent notification, CancellationToken cancellationToken)
    {
        return mediator.Send(new CreateOrActivateDefaultDeckCommand(notification.UserId), cancellationToken);
    }
}
