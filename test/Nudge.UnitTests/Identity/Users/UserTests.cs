using Nudge.Identity.Users.Models;
using Nudge.Identity.Users.ValueObjects;
using Nudge.Shared.Core.BusinessRulesEngine;
using Nudge.Shared.Core.Localization;
using Xunit;

namespace Nudge.UnitTests.Identity.Users;

public class UserTests
{
    [Fact]
    public void Create_When_InputIsValid_Then_ReturnsUserWithMappedFields()
    {
        var id = UserId.Of(1L);
        var telegramUserId = TelegramUserId.Of(987654321L);
        var languageCode = LanguageCode.Of(Language.En);

        var user = User.Create(id, telegramUserId, "Katerina", "Sidorenko", "katrina", languageCode);

        Assert.Equal(id, user.Id);
        Assert.Equal(telegramUserId, user.TelegramUserId);
        Assert.Equal("Katerina", user.FirstName);
        Assert.Equal("Sidorenko", user.LastName);
        Assert.Equal("katrina", user.Username);
        Assert.Equal(languageCode, user.LanguageCode);
    }

    [Fact]
    public void Create_When_OptionalFieldsAreMissing_Then_ReturnsUserWithNullOptionalFields()
    {
        var user = User.Create(UserId.Of(1L), TelegramUserId.Of(1L), "Katerina", lastName: null, username: null, languageCode: LanguageCode.Of(Language.Unknown));

        Assert.Null(user.LastName);
        Assert.Null(user.Username);
    }

    [Fact]
    public void Create_When_FirstNameIsMissing_Then_ThrowsBusinessRuleValidationException()
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            User.Create(UserId.Of(1L), TelegramUserId.Of(1L), string.Empty, null, null, LanguageCode.Of(Language.Unknown)));
    }

    [Fact]
    public void Create_When_TelegramUserIdIsNotPositive_Then_ThrowsBusinessRuleValidationException()
    {
        Assert.Throws<BusinessRuleValidationException>(() =>
            User.Create(UserId.Of(1L), TelegramUserId.Of(0L), "Katerina", null, null, LanguageCode.Of(Language.Unknown)));
    }

    [Fact]
    public void Create_When_UserIsCreated_Then_RaisesUserUpsertedEventWithIsNewUserTrue()
    {
        var user = User.Create(UserId.Of(1L), TelegramUserId.Of(1L), "Katerina", null, null, LanguageCode.Of(Language.Unknown));

        var domainEvent = Assert.Single(user.DomainEvents);
        var userUpsertedEvent = Assert.IsType<UserUpsertedEvent>(domainEvent);
        Assert.Equal(user.Id, userUpsertedEvent.UserId);
        Assert.True(userUpsertedEvent.IsNewUser);
    }

    [Fact]
    public void UpdateProfile_When_InputIsValid_Then_UpdatesProfileFields()
    {
        var user = User.Create(UserId.Of(1L), TelegramUserId.Of(1L), "Katerina", "Sidorenko", "katrina", LanguageCode.Of(Language.En));
        user.ClearDomainEvents();
        var newLanguageCode = LanguageCode.Of(Language.Ru);

        user.UpdateProfile("Kate", null, "kate", newLanguageCode);

        Assert.Equal("Kate", user.FirstName);
        Assert.Null(user.LastName);
        Assert.Equal("kate", user.Username);
        Assert.Equal(newLanguageCode, user.LanguageCode);
    }

    [Fact]
    public void UpdateProfile_When_ProfileIsUpdated_Then_RaisesUserUpsertedEventWithIsNewUserFalse()
    {
        var user = User.Create(UserId.Of(1L), TelegramUserId.Of(1L), "Katerina", null, null, LanguageCode.Of(Language.Unknown));
        user.ClearDomainEvents();

        user.UpdateProfile("Kate", null, null, LanguageCode.Of(Language.Unknown));

        var domainEvent = Assert.Single(user.DomainEvents);
        var userUpsertedEvent = Assert.IsType<UserUpsertedEvent>(domainEvent);
        Assert.False(userUpsertedEvent.IsNewUser);
    }

    [Fact]
    public void UpdateProfile_When_FirstNameIsMissing_Then_ThrowsBusinessRuleValidationException()
    {
        var user = User.Create(UserId.Of(1L), TelegramUserId.Of(1L), "Katerina", null, null, LanguageCode.Of(Language.Unknown));

        Assert.Throws<BusinessRuleValidationException>(() => user.UpdateProfile(" ", null, null, LanguageCode.Of(Language.Unknown)));
    }
}
