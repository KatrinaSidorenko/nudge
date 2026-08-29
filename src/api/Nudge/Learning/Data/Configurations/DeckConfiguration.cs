using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nudge.Learning.Cards.Models;
using Nudge.Learning.Cards.ValueObjects;
using Nudge.Learning.Decks.BussinessRules;
using Nudge.Learning.Decks.Models;
using Nudge.Learning.Decks.ValueObjects;
using Nudge.Shared.EFCore;

namespace Nudge.Learning.Data.Configurations;

public class DeckConfiguration : IEntityTypeConfiguration<Deck>
{
    public void Configure(EntityTypeBuilder<Deck> builder)
    {
        builder.ToTable(EFCoreExtensions.ToTableName<Deck>());

        builder.HasKey(x => x.Id);

        // Id is generated on client-side
        builder.Property(d => d.Id).ValueGeneratedNever()
            .HasConversion<long>(dId => dId.Value, dbId => DeckId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

        // It embeds Title's properties as columns inside the parent entity's database table
        // instead of creating a second relational table with a Foreign Key
        builder.OwnsOne(
            x => x.Title,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(EFCoreExtensions.ToColumnName(() => nameof(Deck.Title)))
                    .HasMaxLength(DeckTitleShouldBeLessThanNCharacters.Length)
                    .IsRequired();
            });

        builder.OwnsOne(
            x => x.Description,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(EFCoreExtensions.ToColumnName(() => nameof(Deck.Description)))
                    .HasMaxLength(DeckDescriptionShouldBeLessThanNCharacters.Length);
            });

        // another way is to make the seperate table with all info
        builder.HasMany(d => d.Cards)
            .WithMany()
            .UsingEntity(
                joinEntityName: EFCoreExtensions.ToTableName<Deck, Card>(),
                configureJoinEntityType: join =>
                {
                    join.HasIndex(nameof(DeckId), nameof(CardId)).IsUnique();
                });
    }
}
