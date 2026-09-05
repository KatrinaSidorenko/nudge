# Nudge — Roadmap

Phases are sequential in intent but not strictly blocking — pull work forward if it unblocks
something else. See [ARCHITECTURE.md](ARCHITECTURE.md) for the decisions behind each item.

## Phase 0 — Foundation

- [ ] Add PostgreSQL to a local `docker-compose.yml` (dev database).
- [ ] Add the first EF Core migrations for the `learning` schema; wire `Database.Migrate()` to run
      on startup (both dev and prod, per architecture decision).
- [ ] Configure `dotnet user-secrets` for local connection strings/secrets; document required env
      vars for containerized runs.
- [ ] Add Serilog (console + rolling file sink), replacing default logging.
- [ ] Add OpenTelemetry SDK with HTTP/gRPC/EF Core auto-instrumentation, console exporter.
- [ ] Add a central exception-handling layer: map `CustomException` /
      `BusinessRuleValidationException` to `ProblemDetails` on REST and to gRPC status codes on
      gRPC.
- [ ] Confirm `.editorconfig`/analyzer warnings are enforced in CI or at build (treat as
      build-breaking where already configured as `warning`).

## Phase 1 — Identity module

- [ ] Create the `Identity` module (`identity` schema): `User` aggregate keyed by
      `TelegramUserId`.
- [ ] Implement Telegram signed-auth verification (shared by both gRPC interceptor and REST login
      endpoint) — this is the one piece of auth logic both transports depend on.
- [ ] gRPC: add an interceptor that verifies Telegram's signed payload per call and resolves the
      caller's `UserId`.
- [ ] REST: implement Telegram Login Widget callback → verify → issue JWT; add JWT bearer auth to
      `Nudge.Api`.
- [ ] Wire a real current-user provider into `AppDbContextBase.OnBeforeSaving` (replaces the
      hardcoded `userId = 0`).
- [ ] Add `UserId` ownership to `Deck` (and cascade to `Card` once it exists); scope all queries by
      the authenticated user.

## Phase 2 — Learning module completion

- [ ] Build out `Card` as a full feature set (Create, Update, Delete, list-by-deck), following the
      `CreateDeck` vertical-slice template exactly.
- [ ] Add Deck management features beyond create: update, archive, delete.
- [ ] Unit tests (xUnit) for `Deck`/`Card` business rules as they're built — per the Phase 0/1
      testing decision, unit tests only, no integration tests yet.

## Phase 3 — Spaced repetition engine

- [ ] Implement SM-2 scheduling state on `Card` (ease factor, interval, repetition count, next
      review date).
- [ ] "Due cards" query, cross-deck (per the confirmed session-composition decision).
- [ ] Review-outcome command (record an answer, recompute next review date via SM-2).
- [ ] Unit tests for the SM-2 implementation — this is the highest-value place for tests in the
      whole system.

## Phase 4 — Telegram bot

- [ ] New project (e.g. `src/bot/Nudge.Bot`) — not yet present in the repo.
- [ ] gRPC client wired to `Nudge.Grpc`, using Telegram's signed auth payload per call.
- [ ] Conversational flow: "Do you remember...?" review sessions, deck/card management commands.
- [ ] Scheduled nudges (push due-card reminders proactively, not just on-demand).

## Phase 5 — Self-hosted deployment

- [ ] `docker-compose.yml` for production: Postgres + `Nudge.Api` + `Nudge.Grpc` + `Nudge.Bot`.
- [ ] Reverse proxy with TLS termination (Caddy or Traefik) in front of REST and gRPC.
- [ ] Env var / Docker secrets wiring for all secrets (no dev user-secrets in prod).
- [ ] Validate Serilog file sink and OTel console exporter behave correctly under Docker.
- [ ] Standard ASP.NET Core rate limiting middleware on public-facing endpoints.

## Phase 6 — Observability hardening

- [ ] Stand up self-hosted Grafana + Tempo + Loki + Prometheus.
- [ ] Swap the OTel exporter from console to this stack.
- [ ] Basic dashboards: request rate/errors/duration, DB query timing.

## Phase 7 — Web UI (future, optional)

- [ ] Frontend stack TBD when this phase starts.
- [ ] Telegram Login Widget web flow against the existing REST JWT auth.
- [ ] Consumes the existing REST API — no new backend auth model needed.

## Phase N — Cloud migration & beyond

- [ ] Lift secrets into a cloud secret store.
- [ ] Move to managed Postgres.
- [ ] Move container hosting to the cloud provider of choice.
- [ ] Swap OTel exporter to the cloud-native equivalent.
- [ ] Revisit: custom domain metrics (cards reviewed/day, review accuracy) once a stats feature is
      actually wanted; integration tests if the domain has grown complex enough to warrant them.
