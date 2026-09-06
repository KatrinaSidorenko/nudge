using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nudge.Learning.Data;

/// <summary>
/// Builds a <see cref="LearningDbContext"/> at design time (e.g. `dotnet ef migrations add`,
/// `dotnet ef database update`) without requiring a running host or a live Postgres connection.
///
/// Configuration is resolved the same way the hosts resolve it at runtime — appsettings.json +
/// appsettings.{Environment}.json (environment from ASPNETCORE_ENVIRONMENT, defaulting to
/// "Development" when unset, matching ASP.NET Core's own convention) + user-secrets — so the
/// connection string a developer set via `dotnet user-secrets` (see README.md) is picked up
/// automatically, per environment. This is the reusable pattern for future modules' first
/// migrations (e.g. Identity).
///
/// Run from the repo root with a startup project so appsettings/user-secrets resolve correctly:
/// `dotnet ef migrations add &lt;Name&gt; --project src/api/Nudge --startup-project src/api/Nudge.Api`.
/// </summary>
public class LearningDbContextFactory : IDesignTimeDbContextFactory<LearningDbContext>
{
    // Matches the <UserSecretsId> in src/api/Nudge.Api/Nudge.Api.csproj. Nudge.Grpc carries the
    // same ConnectionStrings:LearningDb value under its own id (see README.md); either works, this
    // one is just the design-time default.
    private const string NudgeApiUserSecretsId = "eb9747a0-ee3c-4160-ace1-1fd1ab80fda4";

    public LearningDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddUserSecrets(NudgeApiUserSecretsId)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("LearningDb")
            ?? throw new InvalidOperationException(
                "Connection string 'ConnectionStrings:LearningDb' not found. Set it via " +
                "`dotnet user-secrets set \"ConnectionStrings:LearningDb\" \"...\" --project src/api/Nudge.Api` " +
                "(see README.md).");

        var optionsBuilder = new DbContextOptionsBuilder<LearningDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new LearningDbContext(optionsBuilder.Options);
    }
}
