# Branches naming

- `main` - The primary branch for the latest stable code
- `dev` - The branch for the latest development changes
- `feature/<name>` - For new features
- `bugfix/<name>` - For bug fixes
- `hotfix/<name>` - For urgent production fixes
- `task/<name>` - For general tasks like refactoring, documentation, or other non-feature work

<name> - task_id-descriptive_name

Example:
`feature/123_user-authentication`- where `123` is the task ID and `user-authentication` is a descriptive name of the feature being implemented.


- merge from `dev` to `main` with squash commits and a descriptive commit message
- merge from `feature/<name>` to `dev` with a merge commit