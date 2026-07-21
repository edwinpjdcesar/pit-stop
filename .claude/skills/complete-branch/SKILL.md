---
name: complete-branch
description: Wrap up a feature branch after its pull request has merged. Use when the user says they approved and/or merged the PR for the current branch (e.g. "I approved the PR", "PR's merged, finish this branch", "/complete-branch"). Verifies the PR merged, switches to the base branch, pulls latest, and deletes the local branch. Not for creating or reviewing PRs.
---

## When to use

Invoke this when the user tells you they have approved or merged the pull
request created from the branch they are on and want it cleaned up. Typical
phrasings: "I approved the PR", "PR's merged, complete the branch", or an
explicit `/complete-branch`. Do not use it to create or review PRs — only to
wrap one up after it has landed.

## Safety first

Deleting a branch is easy to get wrong. Verify before destroying anything:

1. **Never delete unmerged work.** "Approved" is not "merged" — confirm the
   merge before deleting the local branch.
2. **Never delete `dev` or `main`.** If the current branch is a base branch,
   stop and say so.
3. **Never discard uncommitted changes silently.** If the tree is dirty, stop
   and report it; let the user decide.

## Steps

1. Determine the current branch. If it is a base branch (`dev`/`main`), stop —
   there is nothing to complete.
2. Find the PR for the branch:
   `gh pr view --json number,state,mergedAt,baseRefName,reviewDecision`
3. Act on the state:
   - **Merged** → continue.
   - **Open, approved, not merged** → report it, and offer to merge it now
     (`gh pr merge`). Do not delete until it is merged.
   - **Open, not approved** → report the state and stop.
   - **Closed without merging** → warn that deleting loses the work; stop unless
     the user confirms.
4. Confirm the working tree is clean (`git status --porcelain`). If not, stop.
5. Switch to the PR's base branch (from `baseRefName`, not an assumption) and
   update it: `git switch <base> && git pull --prune`.
6. Delete the local branch safely: `git branch -d <branch>`. If `-d` refuses
   because the PR was squash- or rebase-merged, re-confirm the PR is merged,
   then use `git branch -D <branch>` and note why.
7. Report the outcome: which branch was deleted and that you are now on the base.

## Notes

- Local side only. Enable "Automatically delete head branches" in the repo
  settings to remove the remote branch on merge.
- Respect the PR's base branch in case a PR targeted something other than `dev`.
