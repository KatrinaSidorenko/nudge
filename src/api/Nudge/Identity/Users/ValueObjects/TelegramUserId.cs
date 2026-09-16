namespace Nudge.Identity.Users.ValueObjects;

public record TelegramUserId
{
    public long Value { get; }

    public TelegramUserId(long value)
    {
        Value = value;
    }

    public static TelegramUserId Of(long value)
    {
        return new TelegramUserId(value);
    }

    public static implicit operator long(TelegramUserId id) => id.Value;
}
