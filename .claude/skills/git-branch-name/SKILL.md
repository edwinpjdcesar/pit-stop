---
name: git-branch-name
description: Guide for naming git branches. Use this skill whenever you are creating a new branch. It defines how to name a branch that is clear, consistent, and communicates intent at a glance.
---

## Naming rules

1. **Lowercase only** — no uppercase letters anywhere in the name.
2. **Hyphens to separate words** — use `-` between words, not underscores or spaces.
3. **Alphanumeric characters only** — letters (a–z), digits (0–9), hyphens (`-`), and forward slashes (`/`) for the prefix separator. No special characters, punctuation, or spaces.
4. **No consecutive hyphens** — `feature--new-login` is not allowed.
5. **No trailing hyphen** — do not end the name with `-`.
6. **Descriptive and concise** — the name should reflect the work being done without being vague or overly long.

---

## Required prefix

Every branch must begin with one of the following prefixes followed by a `/`:

| Prefix | Purpose |
|---|---|
| `feature/` | New features or enhancements |
| `bugfix/` | Non-critical bug fixes |
| `hotfix/` | Critical fixes applied directly from production |
| `release/` | Release preparation branches |
| `docs/` | Documentation additions or updates |
| `design/` | Visual or UX changes |
| `refactor/` | Code restructuring with no behavior change |

---

## Descriptive name guidelines

- Briefly describe the primary task or issue the branch addresses.
- Avoid generic, meaningless terms: `update`, `changes`, `stuff`, `misc`, `fix`, `wip`.
- Keep it short — aim for 3–5 words after the prefix.
- Focus on what the branch accomplishes, not how.

---

## Format

```
<prefix>/<short-descriptive-name>
```

---

## Examples

### Feature branches
```
feature/add-user-profile
feature/implement-chat-notifications
feature/login-system
```

### Bug fix branches
```
bugfix/correct-date-display
bugfix/fix-404-error
bugfix/header-styling
```

### Hotfix branches
```
hotfix/critical-security-issue
hotfix/fix-login-issue
hotfix/security-patch
```

### Release branches
```
release/v1.0.1
release/v2.3.0
```

### Documentation branches
```
docs/add-api-instructions
docs/update-contributor-guidelines
docs/api-endpoints
```

### Design branches
```
design/improve-dashboard-ui
design/revise-mobile-layout
```

### Refactor branches
```
refactor/optimize-database-queries
refactor/simplify-api-routes
```

---

## Quick checklist before creating a branch

- [ ] Name is all lowercase
- [ ] Words are separated by hyphens, not underscores or spaces
- [ ] Only alphanumeric characters, hyphens, and `/` prefix separator used
- [ ] No consecutive hyphens (`--`)
- [ ] Name does not end with a hyphen
- [ ] Name starts with a valid prefix (`feature/`, `bugfix/`, `hotfix/`, `release/`, `docs/`, `design/`, `refactor/`)
- [ ] Name is descriptive and concise — avoids vague words like `update` or `changes`
