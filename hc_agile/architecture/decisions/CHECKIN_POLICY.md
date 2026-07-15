# Check-in Policy

Applies to every agent in `hc_agile/team/` and anyone committing code in this project.

## Rule

Before any `git commit` / check-in:

1. **Identify the contributor** — the person/role responsible for the change, based on the "Owns" section of the relevant `hc_agile/team/*_agent.md` file.
2. **Get explicit confirmation from the user** that the identified contributor is correct. Do not commit on assumed or implied confirmation.
3. **Write a proper commit message** — a short subject line plus a description body explaining what changed and why. No bare one-line commit messages.
4. **No Claude/Anthropic attribution** — commits must not include any "Co-Authored-By: Claude" trailer or similar reference.
5. **Blocked contributor values** — the contributor identity used for a commit (author name/email or any "Contributor:" line in the message) must be a real person's name and email from `git config user.name` / `user.email`. The following are never valid contributor values, even if convenient shorthand was used earlier in a conversation:
   - `Claude`, `Claude Code`, `Anthropic`, or any variant/trailer referencing them
   - Abstract role/agent labels such as `Dev .NET Agent`, `Scrum Master Agent`, `Product Owner Agent`, or any `hc_agile/team/*_agent.md` role name used as a stand-in for a person
   - Generic placeholders such as `AI Assistant`, `Automated`, `Bot`, `System`
   If the true contributor is unclear, ask the user by name — do not default to a role or tool label.

## Owner

Enforced by the Scrum Master agent as part of general process discipline (see `hc_agile/team/scrum_master_agent.md`), applicable across all Dev agents.
