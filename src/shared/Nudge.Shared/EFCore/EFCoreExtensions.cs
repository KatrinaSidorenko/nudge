using System.Linq.Expressions;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Nudge.Shared.Core.Model;

namespace Nudge.Shared.EFCore;

public static class EFCoreExtensions
{
    public static string ToTableName<TEntity>()
       => typeof(TEntity).Name;

    public static string ToTableName<TFirst, TSecond>()
        => $"{ToTableName<TFirst>()}{ToTableName<TSecond>()}";

    public static string ToColumnName(Func<string> getProp)
        => getProp() ?? throw new InvalidOperationException();

    // Modules share one assembly (Nudge.csproj), so the plain ApplyConfigurationsFromAssembly
    // overload would pull every module's IEntityTypeConfiguration<T> into each other's DbContext,
    // breaking schema-per-module isolation. Scope discovery to TMarker's namespace instead — pass
    // a module-root marker type (e.g. LearningRoot, IdentityRoot).
    public static ModelBuilder ApplyConfigurationsFromNamespaceOf<TMarker>(this ModelBuilder builder)
    {
        var targetType = typeof(TMarker);
        var targetNamespace = targetType.Namespace;

        if (string.IsNullOrEmpty(targetNamespace))
        {
            throw new InvalidOperationException($"Type '{targetType.Name}' does not have a valid namespace.");
        }

        return builder.ApplyConfigurationsFromAssembly(
            targetType.Assembly,
            type => type.Namespace?.StartsWith(targetNamespace, StringComparison.Ordinal) == true);
    }

    // ref: https://github.com/pdevito3/MessageBusTestingInMemHarness/blob/main/RecipeManagement/src/RecipeManagement/Databases/RecipesDbContext.cs
    public static void FilterSoftDeletedProperties(this ModelBuilder modelBuilder)
    {
        Expression<Func<IAggregate, bool>> filterExpr = e => !e.IsDeleted;

        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes()
                     .Where(m => m.ClrType.IsAssignableTo(typeof(IEntity))))
        {
            // modify expression to handle correct child type
            var parameter = Expression.Parameter(mutableEntityType.ClrType);

            var body = ReplacingExpressionVisitor
                .Replace(filterExpr.Parameters.First(), parameter, filterExpr.Body);

            var lambdaExpression = Expression.Lambda(body, parameter);

            // set filter
            mutableEntityType.SetQueryFilter(lambdaExpression);
        }
    }

    // ref: https://andrewlock.net/customising-asp-net-core-identity-ef-core-naming-conventions-for-postgresql/
    public static void ToSnakeCaseTables(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Replace table names
            entity.SetTableName(entity.GetTableName()?.Underscore());

            var tableObjectIdentifier =
                StoreObjectIdentifier.Table(
                    entity.GetTableName()?.Underscore() !,
                    entity.GetSchema());

            // Replace column names
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName(tableObjectIdentifier)?.Underscore());
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName()?.Underscore());
            }

            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName()?.Underscore());
            }
        }
    }
}
