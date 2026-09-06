# Configuration — local dev env vars

Scope: the environment variables the **local docker-compose Postgres stack** needs for dev, and
how they flow into the two API hosts. This is not a production env-var contract — Phase 5 decides
the real deployment secrets story (Docker secrets vs. env vars, see `ARCHITECTURE.md` §9).

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
`dotnet user-secrets` store (see README "Local development") rather than `appsettings.json`. The
value must match whatever you put in `.env` above:

| Key                              | Purpose                                | Example value                                                            | Consumed by                          |
|----------------------------------|-----------------------------------------|---------------------------------------------------------------------------|----------------------------------------|
| `ConnectionStrings:LearningDb`   | Npgsql connection string, `learning` schema | `Host=localhost;Port=5432;Database=nudge;Username=nudge;Password=changeme` | `dotnet user-secrets` (`Nudge.Api`, `Nudge.Grpc`) |

## From a fresh clone

1. `cp .env.example .env` (adjust values if you like) and `docker compose --env-file .env up -d`.
2. Set `ConnectionStrings:LearningDb` via `dotnet user-secrets set` for both `Nudge.Api` and
   `Nudge.Grpc`, using the same host/port/db/user/password as your `.env`.

No `appsettings.json` edits and no committed secrets required.
