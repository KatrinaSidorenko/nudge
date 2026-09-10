namespace Nudge.Identity.Users.ValueObjects;

public record UserId
{
    public long Value { get; }

    public UserId(long id)
    {
        Value = id;
    }

    public static UserId Of(long id)
    {
        if (id == 0L)
        {
            throw new InvalidOperationException("Something went wrong with id");
        }

        return new UserId(id);
    }

    public static implicit operator long(UserId id) => id.Value;
}
