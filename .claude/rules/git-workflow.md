# Branching & issue rules (non-negotiable)

Full rationale and the complete label taxonomy live in `.github/CONTRIBUTING.md` — this file is
the condensed version for an agent to follow without re-reading that doc every time. If the two
ever disagree, `.github/CONTRIBUTING.md` wins and this file should be updated to match.

## Branching

- `main` — always deployable/stable. Never push to it directly; only reviewed PRs (from `dev`, or
  a `hotfix/*` for urgent production fixes) land here.
- `dev` — the integration branch. Default base for new work.
- Branch name = `<type>/<issue#>-<kebab-slug>`, type is one of `feature`, `fix`, `hotfix`,
  `chore`, `docs`, or `release/<version>` for a release cut. Lowercase, kebab-case, no
  underscores/spaces, slug ≤ 5 words, include the tracker issue number when one exists.
- `feature/*`, `fix/*`, `chore/*`, `docs/*` branch from `dev` and merge back to `dev`.
  `hotfix/*` branches from `main` and merges to **both** `main` and `dev`.
- Squash-merge into `dev`, delete the branch after. `dev → main` only via a reviewed, tagged
  release PR — never merge directly to `main` for routine work.
- Do not rename or "fix" existing branches to match this convention retroactively (e.g. the
  pre-existing `feature/2_create-deck`) — it applies from here forward only.

## Issues

- Title: `<type>: <imperative summary>` (lowercase, no trailing period), Conventional-Commits
  vocabulary — `feat`, `fix`, `chore`, `docs`, `test`, `refactor`. Same vocabulary as branch types
  and eventual commit/PR titles.
- Every issue needs exactly one `type:*` label and exactly one `area:*` label (area names mirror
  `docs/SCOPE.md` section names: `identity`, `decks`, `cards`, `review-engine`, `bot`,
  `quick-capture`, `digest`, `search`, `settings`, `infra`). `priority:*` is optional.
- GitHub's native Issue Types field is **not available** on this repo (personal-account repos
  require an org on Team/Enterprise Cloud) — always use the `type:*` label, never suggest the
  native field exists here.
- When an issue implements a SCOPE.md user story, reference the story number in the issue body
  (e.g. "Implements SCOPE.md story 29") rather than restating the story text.
- `ready-for-agent` is the workflow label `/to-spec` applies when a spec is complete — don't apply
  it manually to an issue that hasn't gone through that process.
- Use the existing issue-form templates (`.github/ISSUE_TEMPLATE/`) rather than freehand issues
  where one of `bug_report`, `feature_request`, or `task` fits.
