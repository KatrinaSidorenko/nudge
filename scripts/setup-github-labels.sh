#!/usr/bin/env bash
# Creates (or updates) this repo's GitHub label taxonomy — see .github/CONTRIBUTING.md.
# Idempotent: safe to re-run. Requires the GitHub CLI (`gh`), authenticated (`gh auth login`)
# with write access to the repo.
set -euo pipefail

REPO="${1:-KatrinaSidorenko/nudge}"

if ! command -v gh >/dev/null 2>&1; then
  echo "error: gh CLI not found. Install it (https://cli.github.com/) and run 'gh auth login' first." >&2
  exit 1
fi

# name|color|description
labels=(
  "type:feat|0e8a16|New feature or enhancement"
  "type:fix|d73a4a|Bug fix"
  "type:chore|c5def5|Tooling, deps, CI, non-product infra"
  "type:docs|0075ca|Documentation-only change"
  "type:test|fbca04|Test-only change"
  "type:refactor|5319e7|Internal restructuring, no behavior change"
  "area:identity|bfd4f2|Accounts & identity module"
  "area:decks|bfd4f2|Deck management"
  "area:cards|bfd4f2|Card management"
  "area:review-engine|bfd4f2|SM-2 scheduling and review sessions"
  "area:bot|bfd4f2|Telegram bot client"
  "area:quick-capture|bfd4f2|Quick capture and Inbox deck"
  "area:digest|bfd4f2|Daily digest"
  "area:search|bfd4f2|Keyword search"
  "area:settings|bfd4f2|Per-user settings"
  "area:infra|bfd4f2|Deployment, observability, secrets"
  "priority:critical|b60205|Drop everything"
  "priority:high|d93f0b|Should be next"
  "priority:medium|fbca04|Normal priority"
  "priority:low|c2e0c6|Nice to have"
  "ready-for-agent|1d76db|Spec is complete; ready for an agent to implement"
)

for entry in "${labels[@]}"; do
  IFS='|' read -r name color description <<< "$entry"
  if gh label list --repo "$REPO" --json name --jq ".[].name" | grep -Fxq "$name"; then
    echo "updating: $name"
    gh label edit "$name" --repo "$REPO" --color "$color" --description "$description"
  else
    echo "creating: $name"
    gh label create "$name" --repo "$REPO" --color "$color" --description "$description"
  fi
done
