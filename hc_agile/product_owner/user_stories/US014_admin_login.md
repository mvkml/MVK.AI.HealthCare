# US014 - Admin Login Page

**As an** Admin
**I want to** log in through a dedicated `/admin/login` URL
**So that** I have a separate entry point into an admin environment, distinct from the
Doctor/Patient login

## Background
Part of the broader Admin epic: the eventual goal is an Admin persona that manages Doctor and
Patient details (record oversight/CRUD) — that management capability is **not** built by this
story. This story is only the entry point: a login page, structurally identical to the existing
`features/auth/pages/login` page, but under an `/admin/...` route so it reads as a separate
environment rather than a third option on the existing Doctor/Patient login.

## Backend status
**Mock only.** No `Admin` role exists in `HC.AI.Identity.Api`'s `Roles` table (simplified to just
`1 Doctor` / `2 Patient`), and no admin-specific endpoints exist. Real backend wiring is future
work, scoped once the actual admin-management capability (US013+?) is defined — see BACKLOG PB031.

## Acceptance Criteria
- [x] `/admin/login` route — separate from `/login`, same visual theme as the existing auth pages
- [x] Email/password fields, loading + error states — same shape as `Login`
- [x] On success, redirects into an admin landing area (`/admin/home`), guarded so it can't be
      reached without an admin session
- [x] Uses a mock auth service (`AdminAuthMockService`) — no real HTTP call, since no backend
      Admin role exists yet

## Explicitly out of scope for this story
- Doctor/Patient record management screens (the actual "admin management" capability) — future
  work, not yet scoped
- Real backend (`HC.AI.Identity.Api` Admin role, admin-specific endpoints) — blocked on that
  scope being defined first

## Priority: High
## Status: Done (mock)
## Sprint: Unscheduled
## Worklogs:
- [20260720_170000_admin_identity_mock.md](../../worklogs/dev_angular/20260720_170000_admin_identity_mock.md)
