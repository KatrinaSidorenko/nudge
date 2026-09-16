using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nudge.Identity.Users.Models;
using Nudge.Identity.Users.ValueObjects;
using Nudge.Shared.EFCore;

namespace Nudge.Identity.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(EFCoreExtensions.ToTableName<User>());

        builder.HasKey(x => x.Id);

        // Id is generated on client-side
        builder.Property(u => u.Id).ValueGeneratedNever()
            .HasConversion<long>(uId => uId.Value, dbId => UserId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

        builder.Property(u => u.FirstName).IsRequired();
        builder.Property(u => u.LastName);
        builder.Property(u => u.Username);
        builder.Property(u => u.LanguageCode);

        // It embeds TelegramUserId's properties as columns inside the parent entity's database
        // table instead of creating a second relational table with a Foreign Key
        builder.OwnsOne(
            x => x.TelegramUserId,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(EFCoreExtensions.ToColumnName(() => nameof(User.TelegramUserId)))
                    .IsRequired();

                // Identity is keyed by Telegram user ID — a duplicate row for the same Telegram
                // user would break user resolution, so this is enforced at the DB level.
                a.HasIndex(p => p.Value).IsUnique();
            });
    }
}
