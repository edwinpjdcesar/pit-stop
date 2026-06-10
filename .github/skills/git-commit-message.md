# Skill: Git Commit Message

Use this skill whenever you are committing changes. It defines how to write a commit message that is clear, consistent, and useful to anyone reading the project history.

---

## When to use a single-line message

If the change is straightforward and needs no further explanation, a single subject line is enough. Use this when the what is obvious and the why doesn't require context.

The subject line must be 50 characters or fewer.

> **Example:** `Fix typo in introduction to user guide`
> **Example:** `Add .editorconfig with default formatting rules`
> **Example:** `Remove unused import in LapController`

If someone wants to know more, they can check the diff. Don't add a body just to fill space.

---

## When to use a subject and body

Add a body when the change needs context — when the *why* isn't obvious from the diff, when a decision was made that a future reader might question, or when the change affects behavior in a way worth calling out.

Follow the **50/72 rule**:

- **Subject line:** 50 characters max
- **Blank line:** required between subject and body
- **Body lines:** 72 characters max each, wrapped manually with hard line breaks

---

## Subject line rules

1. **Start with an imperative verb** — write it as a command: `Add`, `Fix`, `Update`, `Remove`, `Refactor`, `Move`, `Rename`. Not `Added`, `Fixes`, or `Adding`.
2. **Capitalize the first letter.**
3. **No period at the end.**
4. **50 characters or fewer.** If you can't fit the subject, simplify it — don't squeeze more in.
5. **Be specific.** `Update service` is weak. `Update FuelService to handle null input` is better (and still short).

---

## Body rules

1. **Explain what changed and why, not how.** The diff shows the how. The body should give the reader context they can't get from reading the code.
2. **Wrap lines at 72 characters.** Use hard line breaks — don't let the text run long on a single line.
3. **Use plain language.** Short sentences, no filler words, no AI-sounding phrases.
4. **Separate concerns with blank lines** if the body covers multiple distinct points.

---

## Format

```
<subject line — 50 chars max>
<blank line>
<body — 72 chars per line max, wrapped with hard line breaks>
```

---

## Examples

### Single-line (simple change)
```
Fix null reference in lap time formatter
```

### Subject + body (change that needs context)
```
Switch lap cache to session-scoped storage

The previous cache was application-scoped, which meant lap data
from one session could bleed into another under certain load
patterns. Scoping it to the session ID fixes the isolation issue
and makes cache invalidation straightforward.
```

### Subject + body (decision worth documenting)
```
Remove automatic retry logic from FuelClient

Retries were silently masking failures from the external fuel
data API. Callers now receive the error immediately and can
decide how to handle it. A future task will add explicit retry
support with configurable backoff.
```

---

## Quick checklist before committing

- [ ] Subject is 50 characters or fewer
- [ ] Subject starts with an imperative verb, capitalized, no trailing period
- [ ] If a body is included, there is a blank line between subject and body
- [ ] Body lines are 72 characters or fewer (hard-wrapped)
- [ ] The message explains *what* and *why*, not *how*
