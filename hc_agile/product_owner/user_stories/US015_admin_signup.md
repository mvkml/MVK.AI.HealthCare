# US015 - Admin Sign-Up Page

**As an** Admin
**I want to** create an account through a dedicated `/admin/signup` URL
**So that** admin accounts are provisioned through the same kind of flow as Doctor/Patient
accounts, without being mixed into that same page

## Background
Companion story to [US014](US014_admin_login.md) — same Admin Identity feature. Structurally
identical to `features/auth/pages/signup`, minus the persona selector (an admin signup is always
persona = Admin, so there's nothing to choose).

## Backend status
**Mock only** — same reasoning as US014. No `Admin` role exists server-side yet.

## Acceptance Criteria
- [x] `/admin/signup` route — same visual theme as the existing auth pages
- [x] Full name, email, password + confirmation, same validation rules as the existing `Signup`
      page (8+ char password, confirmation match)
- [x] No persona selector — this page only ever creates an Admin account
- [x] Uses the same mock auth service as US014 (`AdminAuthMockService`) — signup succeeds
      client-side only, no real HTTP call

## Explicitly out of scope for this story
- Same exclusions as US014: no Doctor/Patient management screens, no real backend

## Priority: High
## Status: Done (mock)
## Sprint: Unscheduled
## Worklogs:
- [20260720_170000_admin_identity_mock.md](../../worklogs/dev_angular/20260720_170000_admin_identity_mock.md)
