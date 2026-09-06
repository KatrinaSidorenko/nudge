# Branching & Issue Conventions

Enterprise-standard conventions for this repo. Applies from here forward — existing branches
(e.g. `feature/2_create-deck`) are not renamed retroactively.

## Branch model

| Branch | Purpose | Branched from | Merges to |
|---|---|---|---|
| `main` | Always deployable/stable. No direct pushes. | — | — |
| `dev` | Integration branch; default target for day-to-day work. | `main` | `main` (via release PR) |
| `feature/<issue#>-<kebab-slug>` | New feature or enhancement. | `dev` | `dev` |
| `fix/<issue#>-<kebab-slug>` | Bug fix (non-urgent). | `dev` | `dev` |
| `hotfix/<issue#>-<kebab-slug>` | Urgent production fix. | `main` | `main` **and** `dev` |
| `chore/<kebab-slug>` | Tooling, deps, CI, non-product infra. | `dev` | `dev` |
| `docs/<kebab-slug>` | Documentation-only changes. | `dev` | `dev` |
| `release/<version>` | Optional: stabilize a release cut. | `dev` | `main` (tagged) |

**Naming rules**: lowercase, kebab-case, no underscores or spaces. Include the tracker issue
number when one exists (`feature/24-quick-capture-inbox`). Keep the slug short — five words or
fewer.

**Merge rules**: squash-merge into `dev`, delete the branch afterward. `dev → main` only through a
reviewed release PR, tagged on merge. `hotfix/*` merges to `main` first, then is merged/cherry-picked
into `dev` so the fix isn't lost on the next release.

## Issue naming

Title format, [Conventional Commits](https://www.conventionalcommits.org/)-style:

```
<type>: <imperative summary, lowercase, no trailing period>
```

Examples: `feat: add quick-capture to Inbox deck`, `fix: deck archive does not cascade to cards`,
`chore: add docker-compose for postgres`.

Types: `feat`, `fix`, `chore`, `docs`, `test`, `refactor`. Using the same vocabulary for issue
titles, branch slugs, and eventual commit/PR titles means a changelog can be generated later
without inventing a second taxonomy.

Every issue also gets:
- exactly one `type:*` label — see taxonomy below. GitHub's native Issue Types field isn't
  available on this repo (personal-account repos require an org on Team/Enterprise Cloud), so
  labels are the substitute, and the `/to-spec` skill's `ready-for-agent` workflow already expects
  labels rather than that field.
- exactly one `area:*` label, matching a [SCOPE.md](../docs/SCOPE.md) section.
- optionally one `priority:*` label.
- a reference to the SCOPE.md story number(s) it satisfies, in the body — e.g. "Implements
  SCOPE.md story 29."

## Label taxonomy

Created/verified by `scripts/setup-github-labels.sh` (see that script for exact colors).

**Type** (exactly one per issue):
`type:feat`, `type:fix`, `type:chore`, `type:docs`, `type:test`, `type:refactor`

**Area** (exactly one per issue, mirrors SCOPE.md sections):
`area:identity`, `area:decks`, `area:cards`, `area:review-engine`, `area:bot`,
`area:quick-capture`, `area:digest`, `area:search`, `area:settings`, `area:infra`

**Priority** (optional):
`priority:critical`, `priority:high`, `priority:medium`, `priority:low`

**Workflow** (used by agent-driven skills, e.g. `/to-spec`):
`ready-for-agent`
