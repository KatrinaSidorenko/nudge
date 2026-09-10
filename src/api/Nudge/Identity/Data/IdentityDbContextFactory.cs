using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nudge.Identity.Data;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    // Can't use AddUserSecrets<T> here since this project doesn't reference a type from
    // Nudge.Api's assembly — matches Nudge.Api's <UserSecretsId> instead.
    private const string NudgeApiUserSecretsId = "eb9747a0-ee3c-4160-ace1-1fd1ab80fda4";

    public IdentityDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddUserSecrets(NudgeApiUserSecretsId)
            .AddEnvironmentVariables()
            .Build();

        var options = configuration.GetSection(IdentityDbOptions.SectionName).Get<IdentityDbOptions>();
        var connectionString = options?.ConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{IdentityDbOptions.SectionName}:{nameof(IdentityDbOptions.ConnectionString)}' " +
                "not found. Set it via `dotnet user-secrets set` (see README.md).");
        }

        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new IdentityDbContext(optionsBuilder.Options);
    }
}
