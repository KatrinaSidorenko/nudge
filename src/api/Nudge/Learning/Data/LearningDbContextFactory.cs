using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nudge.Learning.Data;

public class LearningDbContextFactory : IDesignTimeDbContextFactory<LearningDbContext>
{
    // Can't use AddUserSecrets<T> here since this project doesn't reference a type from
    // Nudge.Api's assembly — matches Nudge.Api's <UserSecretsId> instead.
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
