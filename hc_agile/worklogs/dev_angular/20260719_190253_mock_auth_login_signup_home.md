# Login / Signup / Home — mock-first (2026-07-19)

Implements [TASK011](../../scrum/tasks/TASK011_US009_mock_auth_ui.md) / US009. Structural
reference: `C:\git\v\ai_hr\hr_ui\aihrweb` (Login/Signup pages) and `AI.HR.Api`'s `UsersController`
— re-themed to this project's design tokens rather than copied, per user instruction. Full path
inventory for the reference was recorded in an earlier worklog when it was first located.

## What was built

- `features/auth/models/auth.model.ts` — `AuthUser`, `Persona` ('doctor' | 'patient'), `RoleOption`, `AuthResult`
- `features/auth/data/auth-mock.service.ts` — `AuthMockService`: two seeded demo accounts
  (`doctor@demo.health` / `patient@demo.health`, password `demo1234`), `login()`/`signUp()` return
  `Observable`s with a simulated 500ms delay, session persisted to `localStorage` (mock user object
  only — no real token; real token-storage strategy is still PB023, not decided here)
- `features/auth/data/auth.guard.ts` — `authGuard` (`CanActivateFn`), redirects to `/login` if not
  mock-logged-in
- `features/auth/pages/login`, `features/auth/pages/signup` — full validation (empty fields,
  password match, min length 8, password-strength meter on signup), loading + error/success states
- `features/home/pages/home` — persona-aware: Doctor gets a link into the existing chat feature;
  Patient gets an honest "not built yet" placeholder rather than a broken/fake link
- `app.routes.ts` — `'' -> /login` redirect; `/home` and `/chat` now both behind `authGuard`
  (previously `/chat` was open to anyone)
- `ChatPage`/`ChatRail` — persona display now reads the real mock-logged-in user
  (`AuthMockService.currentUser()`) instead of the chat feature's own separate hardcoded
  `MOCK_PERSONA` constant, so there's one source of truth for "who's logged in," not two
- Shared `.auth-shell`/`.auth-card`/`.field`/etc. styles added to global `styles.css` (used by both
  Login and Signup, avoids duplicating the same ~150 lines of CSS per page)

## Deliberately not built (per the user's "mock only, don't implement the backend" instruction)
- No real HTTP calls — everything above is `AuthMockService`, in-memory, resets on page reload
- No forgot/reset password pages (lower priority, not requested this pass)
- No real JWT/session — `localStorage` holds a plain mock user object, not a token

## Verification
- `ng build` — clean, `login`/`signup`/`home` now separate lazy chunks (2.94 kB / 4.62 kB / 3.81 kB)
- `ng test` — 27/27 passing across 8 spec files (up from 9/9 — added
  `auth-mock.service.spec.ts`, `auth.guard.spec.ts`, `login.spec.ts`, `signup.spec.ts`,
  `home.spec.ts`)
- Live: dev server picked up all changes via HMR with no errors; confirmed `/`, `/login`,
  `/signup`, `/home` all serve 200 through the running dev server
- **Not done**: no actual browser/screenshot verification (no browser automation tool available
  this session) — recommend clicking through the flow yourself: `/login` (demo credentials shown
  on the page) → `/home` (persona-aware card) → `/chat` (persona badge now reflects the logged-in
  mock user) → log out → confirm `/home` and `/chat` redirect back to `/login`

## References
- [US009](../../product_owner/user_stories/US009_authentication_login_signup_home.md)
- [TASK011](../../scrum/tasks/TASK011_US009_mock_auth_ui.md)
- [BACKLOG.md](../../product_owner/backlog/BACKLOG.md) — PB022 (this work) / PB023 (blocked backend)
