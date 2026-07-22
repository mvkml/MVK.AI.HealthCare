# hc_demo_web — Source Material for PPT + Architecture Design Docs

Two Markdown files, written to be handed to another AI to generate a PowerPoint deck and a
technical/architecture design document. Structured with slide-sized `##` sections and short
bullets rather than long prose, so each `##` maps roughly to one slide.

| File | Purpose | Best used for |
|---|---|---|
| [01_angular_technology_and_ecosystem.md](01_angular_technology_and_ecosystem.md) | What Angular is, its history, current version, rendering models, and how it compares to other UI technologies | PPT presentation — general technology background |
| [02_aihcweb_architecture_design.md](02_aihcweb_architecture_design.md) | This project's actual Angular architecture (`hc_ui/aihcweb`) — containers, components, data flow, decisions, and known gaps | Architecture/technical design document |

## Accuracy note

Angular's release cadence moves fast (a new major roughly every 6 months). Facts through
**Angular 19** are given with confidence; version/feature claims for **Angular 20 and later** are
flagged as unverified and should be checked against [angular.dev](https://angular.dev) before
they go on a slide or into a design doc presented to a client. This project's own installed
version (**Angular 21**, via `@angular/cli@21.2.19`) is a verified fact — confirmed directly from
this repo's `hc_ui/aihcweb/package.json` — not a claim about the wider Angular release timeline.
