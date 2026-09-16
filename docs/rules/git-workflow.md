# Git Workflow & Branching Guidelines

Standardized branching strategy, naming conventions, and Pull Request (PR) merge policies.

---

## 1. Branch Types & Naming Conventions

Format: `<type>/<short-description>` or `<type>/<issue-number>-<description>` (*lowercase, hyphen-separated*).

| Type | Naming Format | Purpose & Stability |
| :--- | :--- | :--- |
| **Main** | `main` | Production-ready code. Highly stable, direct commits forbidden. |
| **Develop** | `dev` | Integration branch for upcoming release. Staging state. |
| **Feature** | `feat/add-telegram-bot` | New feature development. Cut from `develop`. |
| **Bugfix** | `fix/session-timeout` | Fixes for non-prod environments. Cut from `develop`. |
| **Hotfix** | `hotfix/db-connection-leak` | Urgent production fixes. Cut directly from `main`. |
| **Chore** | `chore/db-set-up` | Set up database. |
| **Release** | `release/v1.2.0` | Preparation & QA for production deployment. |

---

## 2. Branching Topology

```text
        (Hotfix)
  +---> [hotfix/1.0.1] ----------------------> (PR) ----------+
  |                                                           |
  |                                                           v
[main] ---------------------------------------------------> [main] (v1.0.1)
  ^                                                           ^
  |                                                           |
  +--- (PR) --- [release/1.0.0] <-----------------------------+
                     ^
                     |
                 [develop] <--- (PR) --- [feat/add-webhook]
                     ^
                     |
                     +--------- (PR) --- [fix/session-timeout]
```

---

## 3. Pull Request & Merging Rules

| Target Branch | Source Branch | Merge Strategy | Requirements |
| :--- | :--- | :--- | :--- |
| `develop` | `feat/*`, `fix/*` | **Squash & Merge** | 1 Approval + CI Passed |
| `main` | `release/*` | **Merge Commit** (`--no-ff`) | QA Sign-off + Tag Version |
| `develop` | `release/*` | **Merge Commit** / **Fast-Forward** | Sync final release fixes |
| `main` & `develop` | `hotfix/*` | **Merge Commit** (`--no-ff`) | Immediate deploy + Backport |

- **PR Naming:** Use conventional commit style (e.g., `feat: add flashcard pagination`, `fix: correct SM-2 interval calculation`).

---