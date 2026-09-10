# Configuration — local dev env vars

Scope: the environment variables the **local docker-compose Postgres stack** needs for dev, and
how they flow into the API hosts and the bot. This is not a production env-var contract — Phase 5
decides the real deployment secrets story (Docker secrets vs. env vars, see `ARCHITECTURE.md` §9).

## `.env` (docker-compose)

Copy `.env.example` to `.env` at the repo root and adjust as needed; `.env` is gitignored and
`docker compose --env-file .env up -d` reads it.

| Variable          | Purpose                                   | Example/default | Consumed by         |
|-------------------|--------------------------------------------|------------------|----------------------|
| `POSTGRES_USER`     | Postgres superuser created on first boot  | `nudge`          | `docker-compose.yml` |
| `POSTGRES_PASSWORD` | Password for `POSTGRES_USER`              | `changeme`       | `docker-compose.yml` |
| `POSTGRES_DB`       | Database created on first boot            | `nudge`          | `docker-compose.yml` |
| `POSTGRES_PORT`     | Host port mapped to the container's 5432  | `5432`           | `docker-compose.yml` |

## `dotnet user-secrets` (host projects)

`Nudge.Api` and `Nudge.Grpc` each read the Postgres connection string from their own
`dotnet user-secrets` store (see README "Local development"), overriding the placeholder value
committed in `appsettings.json` under the `LearningDb` section (`LearningDbOptions.SectionName`).
The value must match whatever you put in `.env` above:

| Key                            | Purpose                                | Example value                                                            | Consumed by                          |
|---------------------------------|-----------------------------------------|---------------------------------------------------------------------------|----------------------------------------|
| `LearningDb:ConnectionString`   | Npgsql connection string, `learning` schema | `Host=localhost;Port=5432;Database=nudge;Username=nudge;Password=changeme` | `dotnet user-secrets` (`Nudge.Api`, `Nudge.Grpc`) |
| `Telegram:BotToken`             | Token for the real Telegram Bot API, from [@BotFather](https://t.me/BotFather) | `123456:ABC-DEF...`                                                        | `dotnet user-secrets` (`Nudge.Bot`)  |

## From a fresh clone

1. `cp .env.example .env` (adjust values if you like) and `docker compose --env-file .env up -d`.
2. Set `LearningDb:ConnectionString` via `dotnet user-secrets set` for both `Nudge.Api` and
   `Nudge.Grpc`, using the same host/port/db/user/password as your `.env`.
3. Set `Telegram:BotToken` via `dotnet user-secrets set` for `Nudge.Bot`, using a token from
   [@BotFather](https://t.me/BotFather).

No `appsettings.json` edits and no committed secrets required.
