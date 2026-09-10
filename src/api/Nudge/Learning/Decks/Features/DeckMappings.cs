using Mapster;
using Nudge.Learning.Decks.Features.CreateDeck.V1;

namespace Nudge.Learning.Decks.Features;

public class DeckMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateDeckRequestDto, CreateDeckCommand>()
            .ConstructUsing(d => new CreateDeckCommand(d.Title, d.Description));

        config.NewConfig<CreateDeckResult, CreateDeckResponseDto>()
            .Map(r => r.Id, r => r.Id);
    }
}
