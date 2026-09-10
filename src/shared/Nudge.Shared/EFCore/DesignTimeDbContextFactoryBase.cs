using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nudge.Shared.EFCore;

// Shared by every module's design-time factory so `dotnet ef migrations add`/`database update`
// work without a running host or a live Postgres connection. Resolves config the same way the
// hosts do at runtime: appsettings.json + appsettings.{ASPNETCORE_ENVIRONMENT}.json + user-secrets.
public abstract class DesignTimeDbContextFactoryBase<TContext, TOptions> : IDesignTimeDbContextFactory<TContext>
    where TContext : DbContext
    where TOptions : class, IConnectionStringOptions, new()
{
    // Can't use AddUserSecrets<T> here since this project doesn't reference a type from
    // Nudge.Api's assembly — matches Nudge.Api's <UserSecretsId> instead.
    private const string NudgeApiUserSecretsId = "eb9747a0-ee3c-4160-ace1-1fd1ab80fda4";

    protected abstract string SectionName { get; }

    protected abstract TContext CreateDbContext(DbContextOptions<TContext> options);

    public TContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddUserSecrets(NudgeApiUserSecretsId)
            .AddEnvironmentVariables()
            .Build();

        var options = configuration.GetSection(SectionName).Get<TOptions>();
        var connectionString = options?.ConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{SectionName}:{nameof(IConnectionStringOptions.ConnectionString)}' " +
                "not found. Set it via `dotnet user-secrets set` (see README.md).");
        }

        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return CreateDbContext(optionsBuilder.Options);
    }
}
