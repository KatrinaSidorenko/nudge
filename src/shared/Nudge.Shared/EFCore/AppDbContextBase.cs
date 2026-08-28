using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Nudge.Shared.Core.Model;

namespace Nudge.Shared.EFCore;

public abstract class AppDbContextBase : DbContext, IDbContext
{
    private readonly ILogger<AppDbContextBase>? _logger;

    // private IDbContextTransaction _currentTransaction;
    protected AppDbContextBase(DbContextOptions options, ILogger<AppDbContextBase>? logger = null)
        : base(options!)
    {
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        NormalizeDateTimeProperties(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex) // can be thrown due to the optimistic approach usage with version
        {
            foreach (var entry in ex.Entries)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);

                if (databaseValues == null)
                {
                    _logger.LogError("The record no longer exists in the database, The record has been deleted by another user.");
                    throw;
                }

                // Refresh the original values to bypass next concurrency check
                entry.OriginalValues.SetValues(databaseValues);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }

    private void OnBeforeSaving()
    {
        try
        {
            foreach (var entry in ChangeTracker.Entries<IAggregate>())
            {
                var isAuditable = entry.Entity.GetType().IsAssignableTo(typeof(IAggregate));
                var userId = /*_currentUserProvider?.GetCurrentUserId() ??*/ 0; // todo: add current user provider

                if (isAuditable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.CreatedBy = userId;
                            entry.Entity.CreatedAt = DateTime.Now;
                            break;

                        case EntityState.Modified:
                            entry.Entity.LastModifiedBy = userId;
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.Version++;
                            break;

                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entry.Entity.LastModifiedBy = userId;
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.IsDeleted = true;
                            entry.Entity.Version++;
                            break;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("try for find IAggregate", ex);
        }
    }

    private static void NormalizeDateTimeProperties(ModelBuilder builder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            value => NormalizeDateTime(value),
            value => NormalizeDateTime(value));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            value => value.HasValue ? NormalizeDateTime(value.Value) : value,
            value => value.HasValue ? NormalizeDateTime(value.Value) : value);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }

    // assume that all dates in db will be in the UTC format
    private static DateTime NormalizeDateTime(DateTime value)
    {
        return value.Kind == DateTimeKind.Unspecified
            ? value
            : DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
    }
}
