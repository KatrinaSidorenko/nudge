using Mapster;
using Nudge.Identity.Users.Features.UpsertUser.V1;

namespace Nudge.Identity.Users.Features;

public class UserMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UpsertUserRequestDto, UpsertUserCommand>()
            .ConstructUsing(d => new UpsertUserCommand(d.TelegramId, d.FirstName, d.LastName, d.Username, d.LanguageCode));

        config.NewConfig<UpsertUserResult, UpsertUserResponseDto>()
            .Map(r => r.Id, r => r.Id)
            .Map(r => r.IsNewUser, r => r.IsNewUser);
    }
}
