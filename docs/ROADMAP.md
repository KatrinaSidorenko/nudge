# Nudge — Roadmap

Phases now track [SCOPE.md](SCOPE.md)'s feature areas directly — each phase below names the
SCOPE section(s) it delivers and the user stories it satisfies, so "done" for a phase means those
stories actually work, not just that some code exists. See [ARCHITECTURE.md](ARCHITECTURE.md) for
the technical decisions behind each item. Phases are sequential in intent but not strictly
blocking — pull work forward if it unblocks something else.

## Phase 0 — Foundation

Infra prerequisites with no product-facing behavior of their own.

- [x] Add PostgreSQL to a local `docker-compose.yml` (dev database).
- [ ] Add the first EF Core migrations for the `learning` schema, applied manually via
      `dotnet ef database update` (not auto-applied on startup — see architecture decision).
- [ ] Configure `dotnet user-secrets` for local connection strings/secrets; document required env
      vars for containerized runs.
- [ ] Add Serilog (console + rolling file sink), replacing default logging.
- [ ] Add OpenTelemetry SDK with HTTP/gRPC/EF Core auto-instrumentation, console exporter.
- [ ] Add a central exception-handling layer: map `CustomException` /
      `BusinessRuleValidationException` to `ProblemDetails` on REST and to gRPC status codes on
      gRPC.
- [ ] Confirm `.editorconfig`/analyzer warnings are enforced in CI or at build (treat as
      build-breaking where already configured as `warning`).

## Phase 1 — Accounts & Identity

Delivers SCOPE §1 (stories 1–2).

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
- [ ] `/start` implicitly creates the `User` — no separate registration/consent step (story 1).
- [ ] Add `UserId` ownership to `Deck` (cascading to `Card` once it exists in Phase 2); scope all
      queries by the authenticated user (story 2) — decks are private, no exceptions in v1.

## Phase 2 — Decks & Cards

Delivers SCOPE §2 (stories 3–10), §3 (stories 11–17), and §6 (stories 29–31). These three land
together because quick-capture and Inbox depend on Card existing, and both depend on Deck's
archive/delete cascade semantics.

- [ ] `Card` as a real aggregate: a card is either a plain note or a front/back Q&A pair — not a
      fixed type; whether it has an answer is just a populated-or-not field, editable at any time
      (stories 11–14).
- [ ] Full Card CRUD: create, list/view, edit, delete (stories 15–17), following the `CreateDeck`
      vertical-slice template exactly.
- [ ] Full Deck CRUD beyond create: list/view, edit, archive, delete (stories 4–6, 9).
- [ ] Archive cascade: archiving a Deck hides its Cards from review too — no independent per-card
      archive state (stories 7–8).
- [ ] Delete cascade: deleting a Deck soft-deletes its Cards (story 10), consistent with archive.
- [ ] Special **Inbox** deck: auto-created per user, protected — cannot be deleted, archived, or
      renamed; only its cards can change (story 31).
- [ ] Quick capture: a plain message to the bot with no active command creates a note-only card
      immediately (story 29); any card created without an explicitly named deck (quick-capture or
      a deck-less `/newcard`) routes to Inbox (story 30).
- [ ] Unit tests (xUnit) for `Deck`/`Card` business rules as they're built — per the architecture's
      testing rule, unit tests only, no integration tests yet.

## Phase 3 — Review Engine

Delivers SCOPE §4 (stories 18–26).

- [ ] Implement SM-2 scheduling state on `Card` (ease factor, interval, repetition count, next
      review date).
- [ ] Due-review sessions: due cards only, scoped per-deck or cross-deck (stories 18–19); grading
      updates SM-2 state.
- [ ] Practice sessions: random cards regardless of due date, scoped per-deck or cross-deck
      (stories 20–21), and **schedule-neutral** — grading never touches SM-2 state (story 22).
- [ ] Binary grading (remembered/forgot), not 4-point.
- [ ] Q&A review flow: show front → user requests reveal → answer shown → then graded
      (reveal-before-grade, story 23).
- [ ] Note-only review flow: full note shown upfront, "remembered" = "still relevant" (story 24).
- [ ] Session cap: fixed default of 20 cards per session in v1 (story 25); offer to continue with
      another batch if more are due (story 26). Not user-configurable yet — see Phase 6.
- [ ] Unit tests for the SM-2 implementation — the highest-value place for tests in the whole
      system.

## Phase 4 — Telegram Bot

Delivers SCOPE §5 (stories 27–28) and §7 (stories 32–33). This is where the bot project itself
gets built — everything in Phases 1–3 is backend-only until this phase gives it a client.

- [ ] New project (e.g. `src/bot/Nudge.Bot`) in the `/bot/` solution folder — not yet present in
      the repo.
- [ ] gRPC client wired to `Nudge.Grpc`, using Telegram's signed auth payload per call.
- [ ] Slash commands for actions (`/newdeck`, `/newcard`, `/review [deck]`, `/practice [deck]`,
      edit/delete/archive variants); inline keyboards for selection steps — deck choice, reveal,
      remembered/forgot grading (story 27–28).
- [ ] Daily digest: proactive push at a fixed time, cross-deck due cards (story 32).
- [ ] Zero-due fallback: if nothing is due that day, offer a practice session instead of silence
      or a bare "nothing due" message (story 33).

## Phase 5 — Self-hosted deployment

Infra, no new product behavior — makes Phases 0–4 actually reachable outside a dev machine.

- [ ] `docker-compose.yml` for production: Postgres + `Nudge.Api` + `Nudge.Grpc` + `Nudge.Bot`.
- [ ] Reverse proxy with TLS termination (Caddy or Traefik) in front of REST and gRPC.
- [ ] Env var / Docker secrets wiring for all secrets (no dev user-secrets in prod).
- [ ] Validate Serilog file sink and OTel console exporter behave correctly under Docker.
- [ ] Standard ASP.NET Core rate limiting middleware on public-facing endpoints.

## Phase 6 — Search & Settings

Delivers SCOPE §8 (stories 34–35) and §9 (stories 36–37) — grouped because SCOPE explicitly scopes
them together as "later phase, not v1," and because Settings needs to exist as one real feature
covering multiple knobs rather than one-off configurability added piecemeal.

- [ ] `Settings` feature on `User`: review session cap (replaces the Phase 3 fixed default of 20,
      story 36) and daily digest time (replaces the Phase 4 fixed time, story 37 — implies
      per-user timezone handling).
- [ ] Keyword search (`/search <keyword>`), cross-deck including Inbox (stories 34–35).

## Phase 7 — Observability hardening

- [ ] Stand up self-hosted Grafana + Tempo + Loki + Prometheus.
- [ ] Swap the OTel exporter from console to this stack.
- [ ] Basic dashboards: request rate/errors/duration, DB query timing.

## Phase 8 — Web UI (future, optional)

Delivers no new SCOPE stories — same feature set as the bot, different client.

- [ ] Frontend stack TBD when this phase starts.
- [ ] Telegram Login Widget web flow against the existing REST JWT auth.
- [ ] Consumes the existing REST API — no new backend auth model needed.

## Phase N — Cloud migration

- [ ] Lift secrets into a cloud secret store.
- [ ] Move to managed Postgres.
- [ ] Move container hosting to the cloud provider of choice.
- [ ] Swap OTel exporter to the cloud-native equivalent.

## Deferred backlog (no phase assigned)

Per SCOPE.md, these have no phase commitment — pull into a numbered phase only if/when
explicitly requested again, not proactively:

- **Stats & progress** (SCOPE §10, stories 38–40): streaks, review counts, retention rate.
- **Sharing/social** (SCOPE §11): deck sharing, public deck library, importing others' decks.
