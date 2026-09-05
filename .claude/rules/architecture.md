# Architecture rules (non-negotiable)

These are locked-in decisions for the Nudge project. Do not change them without the user
explicitly asking to revisit them. Full rationale in `docs/ARCHITECTURE.md`.

## System shape

- **Modular monolith**, one deployable core (`src/api/Nudge`) shared by two host projects:
  `Nudge.Api` (REST) and `Nudge.Grpc` (gRPC). Do not split modules into separate services/repos.
- **Bounded-context modules** live as top-level folders under `src/api/Nudge/` (e.g. `Learning`,
  `Identity`). Each module owns its own EF Core `DbContext` and its own PostgreSQL **schema**
  (schema-per-module, one physical database). Modules reference each other only by foreign key
  (e.g. `UserId`), never by cross-module navigation property or cross-schema join in code.
- **Two API surfaces, distinct roles**: gRPC is the Telegram bot's real transport. REST is for
  admin/manual testing today and the seam for a possible future web UI. Do not collapse these into
  one, and do not add bot-specific logic to the REST surface or vice versa.

## Tech stack — do not substitute without explicit sign-off

- Database: **PostgreSQL**, single database, schema-per-module.
- ORM: **EF Core**. Table/column names are snake_case via `EFCoreExtensions.ToSnakeCaseTables`.
  Soft delete via `IsDeleted` + the global query filter — never hard-delete an aggregate.
- CQRS: the custom `ICommand`/`ICommandHandler` abstractions in `Nudge.Shared.Core.CQRS`. Do not
  introduce a different mediator library.
- Mapping: **Mapster**/`MapsterMapper`.
- Validation: **FluentValidation** for request-shape checks + the custom Business Rules Engine
  (`IBusinessRule`, `BusinessRuleValidator`) for domain invariants. These are two distinct layers —
  do not put domain invariants in a FluentValidation validator, and do not put shape checks
  (required/max-length on a raw DTO) inside a business rule.
- IDs: Snowflake-generated, wrapped in strongly-typed ID value objects (`DeckId`, `CardId`, ...).
  Never expose a raw `long`/`Guid` as a public identifier type.
- Domain modeling: DDD tactical patterns — `Aggregate<TId>`/`Entity<TId>` base classes, private
  setters, construction only via static factory methods (`Deck.Create(...)`-style).
- Logging: **Serilog**, structured. Metrics/traces: **OpenTelemetry**.
- Migrations: EF Core migrations, **auto-applied on startup** in every environment (not
  design-time-only). Never hand-edit a generated migration.

## Identity & auth

- Identity is keyed by **Telegram user ID** — no password store exists or should be added.
- gRPC calls from the bot must be authenticated by **independently verifying Telegram's signed
  auth payload** server-side. Never trust a caller-supplied user ID without verifying the
  signature.
- REST/web auth issues a backend-owned JWT after verifying a Telegram Login Widget payload. Do not
  add a separate credential system (OAuth provider, email/password) without explicit sign-off.
- Nudge is **multi-user from the start** — every module owning user data must scope by `UserId`;
  never assume a single implicit user.

## Deployment assumption

- Self-hosted via Docker Compose first (Postgres + both API hosts + the bot, behind a
  TLS-terminating reverse proxy), designed to be portable to managed cloud infra later. Don't take
  on a cloud-provider-specific dependency without checking this still holds.

## Testing

- Unit tests (xUnit) only, focused on domain logic (aggregates, business rules, the SM-2
  scheduler). No integration tests yet — don't add a Testcontainers/integration-test setup unless
  asked; it was explicitly deferred.
