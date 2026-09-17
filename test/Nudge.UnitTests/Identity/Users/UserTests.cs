using Nudge.Identity.Users.Models;
using Nudge.Identity.Users.ValueObjects;
using Nudge.Shared.Core.BusinessRulesEngine;
using Xunit;

namespace Nudge.UnitTests.Identity.Users;

public class UserTests
{
    [Fact]
    public void Create_When_InputIsValid_Then_ReturnsUserWithMappedFields()
    {
        var id = UserId.Of(1L);
        var telegramUserId = TelegramUserId.Of(987654321L);

        var user = User.Create(id, telegramUserId, "Katerina", "Sidorenko", "katrina", "en");

        Assert.Equal(id, user.Id);
        Assert.Equal(telegramUserId, user.TelegramUserId);
        Assert.Equal("Katerina", user.FirstName);
        Assert.Equal("Sidorenko", user.LastName);
        Assert.Equal("katrina", user.Username);
        Assert.Equal("en", user.LanguageCode);
    }

    [Fact]
    public void Create_When_OptionalFieldsAreMissing_Then_ReturnsUserWithNullOptionalFields()
    {
        var user = User.Create(UserId.Of(1L), TelegramUserId.Of(1L), "Katerina", lastName: null, username: null, languageCode: null);

        Assert.Null(user.LastName);
        Assert.Null(user.Username);
        Assert.Null(user.LanguageCode);
    }

    [Fact]
    public void Create_When_FirstNameIsMissing_Then_ThrowsBusinessRuleValidationException()
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            User.Create(UserId.Of(1L), TelegramUserId.Of(1L), string.Empty, null, null, null));
    }

    [Fact]
    public void Create_When_TelegramUserIdIsNotPositive_Then_ThrowsBusinessRuleValidationException()
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            User.Create(UserId.Of(1L), TelegramUserId.Of(0L), "Katerina", null, null, null));
    }

    [Fact]
    public void UpdateProfile_When_InputIsValid_Then_UpdatesProfileFields()
    {
        var user = User.Create(UserId.Of(1L), TelegramUserId.Of(1L), "Katerina", "Sidorenko", "katrina", "en");

        user.UpdateProfile("Kate", null, "kate", "ru");

        Assert.Equal("Kate", user.FirstName);
        Assert.Null(user.LastName);
        Assert.Equal("kate", user.Username);
        Assert.Equal("ru", user.LanguageCode);
    }

    [Fact]
    public void UpdateProfile_When_FirstNameIsMissing_Then_ThrowsBusinessRuleValidationException()
    {
        var user = User.Create(UserId.Of(1L), TelegramUserId.Of(1L), "Katerina", null, null, null);

        Assert.Throws<BusinessRuleValidationException>(() => user.UpdateProfile(" ", null, null, null));
    }
}
