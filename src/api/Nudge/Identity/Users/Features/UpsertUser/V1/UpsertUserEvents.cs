using Nudge.Shared.Core.Event;

namespace Nudge.Identity.Users.Features.UpsertUser.V1;

// Raised on every /start call, whether it created a new user or refreshed an existing one's
// profile — IsNewUser lets a future subscriber (e.g. "create a default Inbox deck") tell the two
// apart without a second lookup.
public record UserUpsertedEvent(long UserId, bool IsNewUser) : IDomainEvent;
