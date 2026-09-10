using Nudge.Identity.Users.Models;
using Nudge.Identity.Users.ValueObjects;
using Nudge.Shared.Core.BusinessRulesEngine;
using Xunit;

namespace Nudge.Tests.Identity.Users.Models;

public class UserTests
{
    [Fact]
    public void Create_WithRequiredFieldsOnly_ReturnsUserWithNullOptionalFields()
    {
        var id = UserId.Of(1L);
        var telegramUserId = TelegramUserId.Of(123456789L);

        var user = User.Create(id, telegramUserId, "Katerina", lastName: null, username: null, languageCode: null);

        Assert.Equal(id, user.Id);
        Assert.Equal(telegramUserId, user.TelegramUserId);
        Assert.Equal("Katerina", user.FirstName);
        Assert.Null(user.LastName);
        Assert.Null(user.Username);
        Assert.Null(user.LanguageCode);
    }

    [Fact]
    public void Create_WithAllProfileFields_ReturnsUserWithGivenProfile()
    {
        var id = UserId.Of(1L);
        var telegramUserId = TelegramUserId.Of(123456789L);

        var user = User.Create(id, telegramUserId, "Katerina", "Sidorenko", "katerina_s", "en");

        Assert.Equal("Katerina", user.FirstName);
        Assert.Equal("Sidorenko", user.LastName);
        Assert.Equal("katerina_s", user.Username);
        Assert.Equal("en", user.LanguageCode);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    public void Create_WithNonPositiveTelegramUserId_ThrowsBusinessRuleValidationException(long telegramUserId)
    {
        var id = UserId.Of(1L);

        Assert.Throws<BusinessRuleValidationException>(() =>
            User.Create(id, TelegramUserId.Of(telegramUserId), "Katerina", null, null, null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithoutFirstName_ThrowsBusinessRuleValidationException(string? firstName)
    {
        var id = UserId.Of(1L);
        var telegramUserId = TelegramUserId.Of(123456789L);

        Assert.Throws<BusinessRuleValidationException>(() =>
            User.Create(id, telegramUserId, firstName!, null, null, null));
    }

    [Fact]
    public void UpdateProfile_OverwritesAllFourFields()
    {
        var user = User.Create(
            UserId.Of(1L), TelegramUserId.Of(123456789L), "Katerina", "Sidorenko", "katerina_s", "en");

        user.UpdateProfile("Kate", "S.", "kate_s", "uk");

        Assert.Equal("Kate", user.FirstName);
        Assert.Equal("S.", user.LastName);
        Assert.Equal("kate_s", user.Username);
        Assert.Equal("uk", user.LanguageCode);
    }

    [Fact]
    public void UpdateProfile_WithOptionalFieldsOmitted_ClearsPreviousValues()
    {
        var user = User.Create(
            UserId.Of(1L), TelegramUserId.Of(123456789L), "Katerina", "Sidorenko", "katerina_s", "en");

        user.UpdateProfile("Kate", lastName: null, username: null, languageCode: null);

        Assert.Equal("Kate", user.FirstName);
        Assert.Null(user.LastName);
        Assert.Null(user.Username);
        Assert.Null(user.LanguageCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateProfile_WithoutFirstName_ThrowsBusinessRuleValidationException(string? firstName)
    {
        var user = User.Create(UserId.Of(1L), TelegramUserId.Of(123456789L), "Katerina", null, null, null);

        Assert.Throws<BusinessRuleValidationException>(() => user.UpdateProfile(firstName!, null, null, null));
    }
}
