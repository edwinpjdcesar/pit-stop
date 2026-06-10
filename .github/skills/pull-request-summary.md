# Skill: Pull Request Summary

Use this skill whenever you are creating a pull request. It defines how to write the PR description so that it's useful, readable, and honest about what changed.

---

## What this skill covers

Every PR you create must include a written summary of the changes. The depth of that summary should match the size and impact of the changes — a small fix doesn't need an essay, and a large feature doesn't deserve a one-liner.

---

## How to gauge the right level of detail

### Minimal changes (quick fix, typo, config tweak, starter/scaffold files)

Keep it short. A sentence or two is enough. Just say what changed and why, without over-explaining.

> **Example:** Added an `.editorconfig` file with baseline formatting rules for consistent spacing and line endings across editors.

> **Example:** Fixed a typo in the README under the Setup section.

### Moderate changes (a new feature, a refactor, a bug fix with some nuance)

Give a short paragraph. Cover:
- What the change does
- Why it was needed
- Any trade-offs or decisions worth calling out

Don't pad it. If you can say it in three sentences, use three sentences.

### Significant changes (multiple files touched, architecture affected, behavior changes, data migrations, etc.)

Go into real depth. Use sections or bullets to break things down. Cover:
- A plain-English summary of what changed at the top
- The reason for the change and what problem it solves
- A breakdown of the key areas touched (files, systems, layers)
- Any side effects, assumptions, or things a reviewer should pay attention to
- Anything that was intentionally left out and why (if relevant)

---

## Writing rules

These apply regardless of the size of the change:

1. **Write like a person, not a tool.** Avoid phrases like "This PR introduces...", "Leveraging...", "Ensure robust...", "Facilitate...", "Utilize...". Just say what happened.

2. **Use short sentences.** If a sentence runs long, break it up. Readers shouldn't need to re-read a sentence to understand it.

3. **Be specific.** Vague summaries like "made some updates to the service layer" are not useful. Name the thing that changed.

4. **Don't over-document the obvious.** If someone can read the diff and immediately understand it, the summary just needs to confirm intent, not re-explain the code line by line.

5. **Earn every word.** If a sentence doesn't help the reviewer understand the change, cut it.

---

## PR description format

Use this structure when writing the PR description:

```
## Summary

<Write your summary here following the guidelines above.>

## Changes

<Optional. Use a bullet list if there are multiple distinct areas touched. Skip this section for small changes.>

- <Area or file>: <What changed and why>
- <Area or file>: <What changed and why>

## Notes

<Optional. Use this for anything a reviewer should know before looking at the diff — gotchas, follow-ups, open questions, or context that doesn't fit elsewhere.>
```

Only include sections that add value. For a minor fix, just the `## Summary` section is fine.

---

## Examples

### Small change
```
## Summary

Added a `PitStop.slnx` solution file to organize the project for Visual Studio. No code was changed.
```

### Moderate change
```
## Summary

Added basic error handling to the fuel calculation endpoint. It was returning a 500 with no message when the input was missing — now it returns a 400 with a clear error. Also updated the related unit tests to cover both cases.
```

### Significant change
```
## Summary

Reworked how lap time data is stored and retrieved. Previously, lap times were fetched fresh on every request, which caused noticeable slowness on sessions with a lot of laps. Now they're cached per session and only refreshed when new lap data comes in.

## Changes

- `Data/LapRepository`: Added in-memory cache keyed by session ID. Cache is invalidated when a new lap is recorded.
- `Api/Controllers/LapController`: Removed direct DB calls; now reads from the cache layer.
- `Core/Models/LapSession`: Added `LastUpdated` timestamp to support cache invalidation.

## Notes

The cache lives in memory only — a service restart will clear it. That's acceptable for now, but if sessions need to persist across restarts we'll want to look at distributed caching later.
```
