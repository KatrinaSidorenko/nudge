# Backend conventions

Concrete conventions for API routes, database access, and error handling. These describe *how* to
build within the rules in `.claude/rules/architecture.md`. The canonical reference implementation
for all of this is the `CreateDeck` slice:
`src/api/Nudge/Learning/Decks/Features/CreateDeck/V1/`.

## Feature (vertical-slice) structure

Every feature lives under `{Module}/{SubDomain}/Features/{FeatureName}/V{n}/`:

```
Features/
  CreateDeck/
    V1/
      CreateDeckEndpoint.cs   # route + request/response DTOs + FluentValidation validator
      CreateDeckHandler.cs    # ICommand + ICommandHandler implementation
  DeckMappings.cs             # Mapster config for this sub-domain, shared across its features
  DecksApiPaths.cs            # route path constants for this sub-domain
```

- One feature = one folder. A breaking change to a feature gets a new `V{n}` folder — never mutate
  a shipped `V1` in place.
- Route paths are centralized in a `{SubDomain}ApiPaths.cs` static class (see `DecksApiPaths.cs`);
  don't inline route strings in the endpoint.
- The endpoint file owns: the request DTO, the response DTO, the `AbstractValidator<TRequest>`
  for shape checks, and the `IMinimalEndpoint.MapEndpoint` implementation. It maps
  DTO → Command (Mapster), dispatches via the mediator, maps the result → response DTO.
- Every endpoint declares `.HasApiVersion(x.y)` and, once auth exists, its own authorization
  requirement — don't leave a `// todo: add authorization` past Roadmap Phase 1.
- The handler file owns: the `ICommand` record and its `ICommandHandler<TCommand, TResult>`. Domain
  invariants are enforced by calling into the aggregate's factory/behavior methods — the handler
  itself should not contain business logic beyond orchestration (load, call domain method, save).

## Database access

- One `DbContext` per module (e.g. `LearningDbContext`), scoped to that module's own Postgres
  schema via `builder.HasDefaultSchema(...)`. Configurations live in `Data/Configurations/` as
  `IEntityTypeConfiguration<T>` classes, auto-discovered via
  `ApplyConfigurationsFromAssembly`.
- Always call `builder.FilterSoftDeletedProperties()` and `builder.ToSnakeCaseTables()` in
  `OnModelCreating` for a new `DbContext` (see `LearningDbContext`) — don't hand-roll naming or
  soft-delete filtering per entity.
- Soft delete only: an aggregate delete sets `IsDeleted = true` via the base `SaveChangesAsync`
  interception in `AppDbContextBase`; never issue a hard `DELETE`. Use `.IgnoreQueryFilters()`
  explicitly on the rare query that needs deleted rows.
- Optimistic concurrency is automatic via the `Version` column bump in `AppDbContextBase`; don't
  add a separate concurrency token.
- No repositories — handlers query the `DbContext` directly, scoped to their own module. Do not
  reach into another module's `DbContext` or table; cross-module data needs go through a foreign
  key (`UserId`, etc.), not a join.
- Migrations are applied manually via `dotnet ef database update` (see `docs/ROADMAP.md` Phase 0
  and `.claude/rules/architecture.md`) — not auto-applied on startup. Don't wire
  `Database.Migrate()` into a host's startup path without revisiting this rule first.

## Domain invariants vs. request validation

- **Request shape** (required fields, max length, format) → `AbstractValidator<TRequestDto>`
  (FluentValidation), wired via `.WithValidation<TRequestDto>()`. Failure → 400 via
  `ValidationFilter`.
- **Domain invariants** (business rules that must hold regardless of caller) →
  `IBusinessRule` implementations validated with `BusinessRuleValidator.Validate(...)` inside the
  aggregate's factory/behavior method (see `DeckTitleShouldBeLessThanNCharacters`). Failure →
  `BusinessRuleValidationException`.
- Never duplicate a check in both places — decide which layer owns it and put it there once.

## Error handling

- Domain/application errors are exceptions: `CustomException` (carries an explicit
  `HttpStatusCode`) or `BusinessRuleValidationException`. Throw, don't return error codes/null.
- A central exception-handling layer (Roadmap Phase 0) is responsible for turning these into:
  - REST: a `ProblemDetails` response with the exception's status code.
  - gRPC: the equivalent `RpcException`/`StatusCode`.
- Don't catch-and-swallow in handlers; let exceptions propagate to the central handler. Only catch
  where you're adding context or handling a specific recoverable case (see the
  `DbUpdateConcurrencyException` retry in `AppDbContextBase.SaveChangesAsync` for the pattern).

## Auth (once Phase 1 lands)

- gRPC: a server interceptor verifies Telegram's signed auth payload per call and resolves
  `UserId` before the handler runs — handlers receive an already-authenticated user context, they
  never verify auth themselves.
- REST: standard JWT bearer middleware; handlers receive the authenticated `UserId` the same way.
- Every query/command that touches user-owned data must filter/assign by the authenticated
  `UserId` — there is no "current user" default of a single implicit user.

## Style

- Follow `src/.editorconfig` as-is (StyleCop-backed rules, 4-space indent, `warning`-level
  accessibility modifier requirement, etc.) — don't override or suppress a rule without a specific
  reason called out in the change.
- Comment sparingly: a comment is for a choice the code can't explain itself — a non-obvious
  business rule, a workaround for a library/framework quirk, a deliberate deviation from the
  obvious approach (see the `DbUpdateConcurrencyException` handling in `AppDbContextBase` for the
  pattern). Don't narrate what a change does or add a comment restating what the next line already
  says in code — if a reviewer would ask "why," write a comment; if they'd just read the code,
  don't.
