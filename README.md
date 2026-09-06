# nudge

## Local development

Copy `.env.example` to `.env` and adjust values as needed, then bring up the dev database:

```
cp .env.example .env
docker compose --env-file .env up -d
```

See `docker-compose.yml`.

### Connection string (user-secrets)

Both host projects (`Nudge.Api`, `Nudge.Grpc`) read the Postgres connection string from
`dotnet user-secrets` rather than `appsettings.json`. Each already has its own `UserSecretsId`
wired in its `.csproj`; you only need to set the value, matching whatever credentials you put in
`.env` above (defaults shown):

```
dotnet user-secrets set "ConnectionStrings:LearningDb" "Host=localhost;Port=5432;Database=nudge;Username=nudge;Password=changeme" --project src/api/Nudge.Api
dotnet user-secrets set "ConnectionStrings:LearningDb" "Host=localhost;Port=5432;Database=nudge;Username=nudge;Password=changeme" --project src/api/Nudge.Grpc
```

See `docs/CONFIGURATION.md` for the full list of local env vars this stack uses.

### Database migrations

Migrations are applied manually — never auto-applied on startup. With the dev database up and
`ConnectionStrings:LearningDb` set (above), apply the `learning` schema's migrations:

```
dotnet ef database update --project src/api/Nudge --startup-project src/api/Nudge.Api
```

To add a new migration after changing `LearningDbContext`'s model:

```
dotnet ef migrations add <Name> --project src/api/Nudge --startup-project src/api/Nudge.Api --output-dir Learning/Data/Migrations
```

Both commands work without a running host — `LearningDbContextFactory` (a design-time
`IDesignTimeDbContextFactory<LearningDbContext>`) resolves the connection string from
`appsettings.json` + `appsettings.{ASPNETCORE_ENVIRONMENT}.json` (defaults to `Development`) +
user-secrets, same as the hosts do at runtime. Never hand-edit a generated migration file.

## Credits:
    - https://github.com/meysamhadeli/booking-microservices
    - https://github.com/evolutionary-architecture/evolutionary-architecture-by-example