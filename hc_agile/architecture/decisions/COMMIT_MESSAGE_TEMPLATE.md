# Commit Message Template

Reusable skeleton for every commit in this repo, per the Check-in Policy. Copy the
structure below, fill in the placeholders, keep the Contributor block byte-for-byte.

```
<Short imperative summary, under ~70 chars — what changed, not why>

<Body: what was built/changed and why, in a few short paragraphs or a bullet
list per component/layer touched. Call out anything notable: bugs found and
fixed, decisions made, things verified (build/tests/live endpoint checks),
and anything deliberately left as a stub/TODO with the reason.>

Contributor: Vishnu Kiran M <mvkwithmath@gmail.com>
Azure AI Solution Architect specializing in Azure AI Foundry, Azure OpenAI,
AI Agents, Retrieval-Augmented Generation (RAG), Document Intelligence,
Azure AI Search, API Management, Terraform, AIOps, and enterprise
integration. Experienced in architecting scalable, secure,
production-ready AI solutions on Microsoft Azure.
```

## Rules

- The `Contributor:` line + full bio paragraph is **mandatory on every commit** — this
  repo is treated as a recruiter/portfolio-facing artifact.
- Summary line: imperative mood ("Add", "Fix", "Build out"), no trailing period.
- Body explains **why**, not just what — the diff already shows what.
- Only commit when the user explicitly asks, and only after they've confirmed the
  message (see `CHECKIN_POLICY.md`, not tracked in git per user request).
