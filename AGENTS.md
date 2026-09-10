# AGENTS.md

Instructions for AI coding agents working in this repository. See `docs/ARCHITECTURE.md` for the
full design, `docs/SCOPE.md` for the product's user stories/features, and `docs/ROADMAP.md` for
what's built vs. planned. `.claude/rules/architecture.md` and `.claude/rules/backend.md` are
non-negotiable — read them before making structural or convention decisions.

## What this is

Nudge is a personal spaced-repetition/reminder tool, multi-user from the start. A user creates
Decks of Cards (a card is either a plain note or a front/back Q&A pair); cards are scheduled via
SM-2 and surfaced through a Telegram bot for review. Two API surfaces sit over one domain core:
gRPC (the bot's real transport) and REST (admin/future web).

v1 product scope (full detail and user stories in `docs/SCOPE.md`): Deck/Card CRUD; due (SM-2)
and schedule-neutral practice review sessions, each scopable per-deck or cross-deck, capped per
session; a daily due-cards digest; frictionless quick-capture (any plain message to the bot
becomes a note in a protected, undeletable "Inbox" deck); slash commands for actions, inline
keyboards for selections. Explicitly **not** v1 (later phase or deferred — don't build ahead of
scope): keyword search, per-user settings (session cap/digest time), stats/streaks, deck
sharing.

## Repo layout

```
src/
  Nudge.slnx              # solution: /api, /bot (reserved, empty), /shared folders
  api/
    Nudge.Api/             # REST host
    Nudge.Grpc/             # gRPC host (bot transport)
    Nudge/                   # domain + application core (modules: Learning, Identity planned)
  shared/
    Nudge.Shared/            # cross-cutting kernel: CQRS, DDD base types, EF Core conventions,
                              # business rules engine, minimal-API helpers, Mapster, OpenAPI
docs/                        # ARCHITECTURE.md, SCOPE.md, ROADMAP.md
.claude/rules/                # architecture.md, backend.md — read these first
```

There is no test project yet (see Roadmap Phase 0-2 — unit tests via xUnit are planned but not
scaffolded). There is no `Nudge.Bot` project yet — the `/bot/` solution folder is reserved for it.

## Build / run

```sh
dotnet build src/Nudge.slnx
dotnet run --project src/api/Nudge.Api      # REST host, Swagger at /swagger in Development
dotnet run --project src/api/Nudge.Grpc     # gRPC host
```

No database, migrations, or `docker-compose.yml` exist yet (Roadmap Phase 0). Don't assume a
running Postgres instance unless you've just set one up in the same task.

## Conventions to follow

- **Vertical-slice features**: new work under a module goes in
  `{Module}/{SubDomain}/Features/{FeatureName}/V{n}/`, mirroring
  `src/api/Nudge/Learning/Decks/Features/CreateDeck/V1/` exactly (endpoint + DTOs + validator in
  one file, command + handler in another). Full detail in `.claude/rules/backend.md`.
- **DDD aggregates**: private setters, construction via static `Create(...)` factories, domain
  invariants enforced through `IBusinessRule`/`BusinessRuleValidator`, not inline `if`/`throw`.
- **Two validation layers, don't mix them**: FluentValidation for request shape, the business
  rules engine for domain invariants.
- **EF Core**: one `DbContext` per module, own Postgres schema, snake_case naming and soft-delete
  filtering applied via `EFCoreExtensions` — never hand-roll these per entity, never hard-delete.
- **IDs**: Snowflake-generated, wrapped in strongly-typed ID value objects — never a raw
  `long`/`Guid` as a public identifier.
- **Style**: `src/.editorconfig` (StyleCop-backed) is authoritative — don't suppress a rule
  without calling it out explicitly in the change.
- Match the density and idiom of surrounding code; don't introduce a new library or pattern where
  an existing one in `Nudge.Shared` already covers it.

## Known gaps (don't be surprised by these)

- Auth doesn't exist yet (`// todo: add authorization` in `CreateDeckEndpoint.cs`); the current
  user is hardcoded to `0` in `AppDbContextBase`. Both are Roadmap Phase 1 (Identity module +
  Telegram-signed auth) — there is no `User` entity, `UserId` ownership on `Deck`/`Card`, or
  Identity module yet.
- `Card` is a stub with no fields or features yet — none of the note/Q&A, CRUD, or review-engine
  behavior in `docs/SCOPE.md` is implemented.
- Nothing from `docs/SCOPE.md` is built yet beyond `Deck` creation — treat that file as the spec
  source for upcoming work, not a description of current behavior.
- The `Learning` module's business-rules folder is spelled `BussinessRules` (typo, left as-is
  until touched — spell new modules' equivalent correctly).
