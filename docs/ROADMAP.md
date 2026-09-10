# Nudge — Roadmap

Phases track [SCOPE.md](SCOPE.md)'s feature areas directly — each phase below names the SCOPE
section(s) it delivers and the user stories it satisfies, so "done" for a phase means those
stories actually work, not just that some code exists. See [ARCHITECTURE.md](ARCHITECTURE.md) for
the technical decisions behind each item.

**Except for Phase 0, every phase is an end-to-end vertical slice**: it isn't "done" until a real
user can trigger it from the Telegram bot and see it work all the way through gRPC → handler →
Postgres and back — not just until the backend code exists. Build each story bot-command-first
(or job-first, for the digest), wire the minimum backend it needs, and ship that one thing working
before moving to the next. Don't build a whole module's backend and leave the bot side for a later
phase — that's the layer-by-layer approach this roadmap deliberately replaces. Phases are
sequential in intent but not strictly blocking — pull work forward if it unblocks something else.

## Phase 0 — Foundation

Infra prerequisites with no product-facing behavior of their own.

- [x] Add PostgreSQL to a local `docker-compose.yml` (dev database).
- [x] Add the first EF Core migrations for the `learning` schema, applied manually via
      `dotnet ef database update` (not auto-applied on startup — see architecture decision).
- [x] Configure `dotnet user-secrets` for local connection strings/secrets; document required env
      vars for containerized runs.
- [ ] Add Serilog (console + rolling file sink), replacing default logging.
- [ ] Add OpenTelemetry SDK with HTTP/gRPC/EF Core auto-instrumentation, console exporter.
- [ ] Add a central exception-handling layer: map `CustomException` /
      `BusinessRuleValidationException` to `ProblemDetails` on REST and to gRPC status codes on
      gRPC.
- [ ] Confirm `.editorconfig`/analyzer warnings are enforced in CI or at build (treat as
      build-breaking where already configured as `warning`).

## Phase 1 — Walking skeleton: Identity + Bot + first Deck slice

The point of this phase is to prove the *entire* pipeline once — Telegram → bot → gRPC (real
auth, not a stub) → handler → Postgres → back to the user — on the smallest possible feature,
before building any further breadth. Everything after Phase 1 reuses this skeleton instead of
re-proving it.

Delivers SCOPE §1 (stories 1–2) and the create half of §2 (story 3).

- [x] New project `src/bot/Nudge.Bot` in the `/bot/` solution folder (not yet present in the
      repo), running against the real Telegram Bot API (long polling is fine for local dev).
- [ ] gRPC client in the bot wired to `Nudge.Grpc`.
- [x] Create the `Identity` module (`identity` schema): `User` aggregate keyed by
      `TelegramUserId`.
- [ ] Implement Telegram signed-auth verification once, shared by the `/start` handler now and
      the REST JWT login flow later (Phase 6+) — this is the one piece of auth logic every
      transport depends on.
- [ ] `/start` independently verifies Telegram's signed auth payload and creates-or-updates the
      `User` (profile fields synced from Telegram on every `/start`) — no separate
      registration/consent step (story 1). **Interim**: every other gRPC call trusts an unsigned
      `TelegramUserId` from the bot without verifying it — see the "Identity & auth" interim
      note in `.claude/rules/architecture.md`; a real per-call interceptor is deferred.
- [ ] Wire a real current-user provider into `AppDbContextBase.OnBeforeSaving` (replaces the
      hardcoded `userId = 0`), backed by the unverified `TelegramUserId` above for now.
- [ ] Add `UserId` ownership to `Deck`; scope queries by the authenticated user (story 2) — decks
      are private, no exceptions in v1.
- [ ] **End-to-end acceptance**: `/newdeck` in the bot creates a real `Deck` row, owned by the
      calling Telegram user, and the bot confirms it back in chat (story 3, create only — the
      rest of Deck CRUD is Phase 2).

## Phase 2 — Decks & Cards, full CRUD

Builds out the rest of §2 (stories 4–10), §3 (stories 11–17), and §6 (stories 29–31) on top of the
Phase 1 skeleton — quick-capture and Inbox land here too since both depend on `Card` existing and
on Deck's archive/delete cascade semantics. Each bullet is its own bot-command-to-database slice;
ship and verify one before starting the next rather than batching all the handlers first.

- [ ] Deck: list/view, edit, archive, delete bot commands + handlers (stories 4–6, 9).
- [ ] Archive cascade: archiving a Deck hides its Cards from review too — no independent per-card
      archive state (stories 7–8).
- [ ] Delete cascade: deleting a Deck soft-deletes its Cards (story 10), consistent with archive.
- [ ] `Card` as a real aggregate: a card is either a plain note or a front/back Q&A pair — not a
      fixed type; whether it has an answer is just a populated-or-not field, editable at any time
      (stories 11–14).
- [ ] Card: create, list/view, edit, delete bot commands + handlers (stories 15–17), following the
      `CreateDeck` vertical-slice template exactly.
- [ ] Special **Inbox** deck: auto-created per user, protected — cannot be deleted, archived, or
      renamed; only its cards can change (story 31).
- [ ] Quick capture: a plain message to the bot with no active command creates a note-only card
      immediately (story 29); any card created without an explicitly named deck (quick-capture or
      a deck-less `/newcard`) routes to Inbox (story 30).
- [ ] Unit tests (xUnit) for `Deck`/`Card` business rules as they're built — per the architecture's
      testing rule, unit tests only, no integration tests yet.

## Phase 3 — Review Engine

Delivers SCOPE §4 (stories 18–26). The review session is fundamentally a bot conversation (inline
keyboards for reveal/grading), so build the SM-2 handler and its bot flow together per session
type rather than the scheduler first and a bot UI later.

- [ ] SM-2 scheduling state on `Card` (ease factor, interval, repetition count, next review date).
- [ ] Due-review, cross-deck: `/review` with no deck named, end-to-end through grading
      (story 19); grading updates SM-2 state.
- [ ] Due-review, per-deck: `/review <deck>` (story 18) — same handler, scoped query.
- [ ] Practice sessions, per-deck and cross-deck: `/practice [deck]` (stories 20–21),
      **schedule-neutral** — grading never touches SM-2 state (story 22).
- [ ] Binary grading (remembered/forgot), not 4-point.
- [ ] Q&A review flow: show front → user requests reveal → answer shown → then graded
      (reveal-before-grade, story 23).
- [ ] Note-only review flow: full note shown upfront, "remembered" = "still relevant" (story 24).
- [ ] Session cap: fixed default of 20 cards per session in v1 (story 25); offer to continue with
      another batch if more are due (story 26). Not user-configurable yet — see Phase 6.
- [ ] Unit tests for the SM-2 implementation — the highest-value place for tests in the whole
      system.

## Phase 4 — Daily Digest

Delivers SCOPE §7 (stories 32–33). The bot foundation and the review-session flow it reuses
already exist from Phases 1–3, so this phase is just the proactive trigger.

- [ ] Scheduled job that pushes a digest at a fixed time to every user with due cards, cross-deck
      (story 32), reusing the Phase 3 due-review flow rather than a separate code path.
- [ ] Zero-due fallback: if nothing is due that day, offer a practice session instead of silence
      or a bare "nothing due" message (story 33).

## Phase 5 — Self-hosted deployment

Infra, no new product behavior — makes Phases 0–4 reachable outside a dev machine so the product
built so far can actually be used day-to-day, not just tested locally.

- [ ] `docker-compose.yml` for production: Postgres + `Nudge.Api` + `Nudge.Grpc` + `Nudge.Bot`.
- [ ] Reverse proxy with TLS termination (Caddy or Traefik) in front of REST and gRPC.
- [ ] Env var / Docker secrets wiring for all secrets (no dev user-secrets in prod).
- [ ] Validate Serilog file sink and OTel console exporter behave correctly under Docker.
- [ ] Standard ASP.NET Core rate limiting middleware on public-facing endpoints.

## Phase 6 — Search & Settings

Delivers SCOPE §8 (stories 34–35) and §9 (stories 36–37) — grouped because SCOPE explicitly scopes
them together as "later phase, not v1," and because Settings needs to exist as one real feature
covering multiple knobs rather than one-off configurability added piecemeal. As with every phase
above, each ships as a bot command wired straight to its handler, not backend-then-bot.

- [ ] `Settings` feature on `User`: review session cap (replaces the Phase 3 fixed default of 20,
      story 36) and daily digest time (replaces the Phase 4 fixed time, story 37 — implies
      per-user timezone handling), exposed via a `/settings` bot command.
- [ ] Keyword search (`/search <keyword>`), cross-deck including Inbox (stories 34–35).
- [ ] REST/web auth: Telegram Login Widget callback → verify (reusing the Phase 1 signed-auth
      check) → issue JWT; add JWT bearer auth to `Nudge.Api`. Pulled in here rather than earlier
      because nothing has needed REST auth until a web-facing surface does (Phase 8).

## Phase 7 — Observability hardening

- [ ] Stand up self-hosted Grafana + Tempo + Loki + Prometheus.
- [ ] Swap the OTel exporter from console to this stack.
- [ ] Basic dashboards: request rate/errors/duration, DB query timing.

## Phase 8 — Web UI (future, optional)

Delivers no new SCOPE stories — same feature set as the bot, different client.

- [ ] Frontend stack TBD when this phase starts.
- [ ] Telegram Login Widget web flow against the Phase 6 REST JWT auth.
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
