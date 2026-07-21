# CLAUDE.md

## Preferences
When providing an explanation, be clear and concise. Avoid using filler words or phrases. Use proper grammar and punctuation.

When providing code examples, ensure that the code is correct, complete, and follows best practices for the given programming language. Include comments in the code to explain complex logic or important details. Always present them in a code block with the appropriate syntax highlighting for the language.

## Skill Usage
When suggesting changes to existing code, provide a clear rationale for the changes and explain how they improve the code's functionality, readability, or maintainability.
When making a git commit, always follow the `git-commit-message` skill (`.claude/skills/git-commit-message`).

When creating a pull request, always follow the `pull-request-summary` skill (`.claude/skills/pull-request-summary`).

When asked to write unit tests for any language (including .NET, Java, JavaScript/TypeScript, Python, Go, or any language following Arrange-Act-Assert patterns), always follow the `unit-testing` skill (`.claude/skills/unit-testing`).

When creating a new branch, always follow the `git-branch-name` skill (`.claude/skills/git-branch-name`).

## Frontend
When making changes to React code, always run `npm run build` inside the `frontend/` directory to validate the project builds without errors before considering the task complete.

## Git Workflow
This project follows Git Flow. `feature/*` and other working branches target `dev`. When creating a branch or opening a pull request, branch from `dev` and set `dev` as the PR base unless the user explicitly directs otherwise.

