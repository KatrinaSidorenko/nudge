# AGENTS.md

Reference index for agents working in this repo. Read the linked docs before making non-trivial changes.

## What this is

Nudge — a spaced-repetition flashcard system delivered via Telegram bot. .NET 10 solution, modular monolith with bounded contexts.

## Rules & standards

- [docs/rules/codestyle.md](docs/rules/codestyle.md) — comment policy, StyleCop via `.editorconfig`.
- [docs/rules/git-workflow.md](docs/rules/git-workflow.md) — branch naming, PR/merge strategy per target branch.
- [docs/unit-tests.md](docs/unit-tests.md) — xUnit, `<Scenario>_When_<Action>_Then_<Expected>` naming (no `Given_`).
- [docs/nf-requirments.md](docs/nf-requirments.md) — non-functional requirements (observability, testing strategy, perf/scalability targets).


## Setup / running

See [README.md](README.md) for local dev DB (docker compose), user-secrets (connection string, Telegram bot token), and EF Core migration commands.
