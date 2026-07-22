# Admin Identity — Login + Sign-Up Pages, Mock-First (2026-07-20)

Implements [TASK015](../../scrum/tasks/TASK015_US014_US015_admin_identity_mock.md) /
[US014](../../product_owner/user_stories/US014_admin_login.md) /
[US015](../../product_owner/user_stories/US015_admin_signup.md) / BACKLOG PB031.

## Numbering collision found and resolved before writing any docs
Drafted the two new stories as US011/US012 first. Before writing code, `git status` showed
concurrent work (outside this session) had already claimed both numbers for an unrelated thread —
`US011_persona_prompt_type_and_llm_option_config.md`, `US011_qa_auth_ui_test_coverage.md`,
`US012_dynamic_classification_model_routing.md`, `US013_dynamic_executor_model_prompt_resolution.md`
— all part of EPIC001 (BACKLOG PB032). Renumbered mine to **US014**/**US015** before creating any
Angular files, task file, or Azure DevOps work items, so nothing downstream had to be re-touched.
Same pattern as the earlier TASK009 collision this session: investigate first, renumber the newer
work, don't overwrite what's already there.

## What was built
Mock-first, mirroring the shape `features/auth/` had before `HC.AI.Identity.Api` existed — because
no `Admin` role exists in the real backend (`Roles` table is `1 Doctor` / `2 Patient` only) and no
admin-management endpoints exist to call.

New feature folder `features/admin-auth/`:
- `models/admin-auth.model.ts` — `AdminUser`, `AdminAuthResult`
- `data/admin-auth-mock.service.ts` — `AdminAuthMockService`. Pure client-side: accounts stored in
  `localStorage` (`hc_admin_accounts`), session in `localStorage` (`hc_admin_auth_session`,
  separate key from the real Doctor/Patient session so the two don't collide). `signUp()` rejects
  duplicate emails; `login()` checks email+password against stored accounts (not "accept
  anything") so the signup→login round trip is actually meaningful in a demo
- `data/admin-auth.guard.ts` — `adminAuthGuard`, same shape as `authGuard`, redirects to
  `/admin/login`
- `pages/admin-login/` — mirrors `Login` (email/password, show/hide, loading/error states),
  redirects to `/admin/home` on success
- `pages/admin-signup/` — mirrors `Signup` minus the persona selector (always Admin), same
  password-strength meter and validation rules
- `pages/admin-home/` — minimal guarded landing stub ("Doctor & Patient management coming soon,
  not yet scoped — see PB031"), so the login flow lands somewhere real instead of nowhere; not a
  new user story, just what a working login needs to redirect to

Routes added to `app.routes.ts`: `/admin/login`, `/admin/signup` (no guard, same as `/login`
`/signup`), `/admin/home` (guarded by `adminAuthGuard`).

## Verification
- `ng build` — clean
- `ng test` — 58/58 passing (up from 40) — 18 new tests: 6 service (signup/login round trip,
  duplicate email, logout, session restore from localStorage on a fresh injector), 2 guard
  (allow/redirect), 3 login page, 5 signup page, 2 home page
- Live, through the running dev server: `GET http://localhost:4200/admin/login` and
  `/admin/signup` both return 200

## Azure DevOps work items created
First PB item to actually go through TASK014's intended Epic > Feature > User Story mirroring
structure (Epic #1/Issue #2 from earlier were generic process items, not a PB mirror):
- Epic #44 — "Admin Management"
- Feature #45 — "Admin Identity" (parent: Epic #44)
- User Story #46 — "Admin Login Page" (parent: Feature #45)
- User Story #47 — "Admin Sign-Up Page" (parent: Feature #45)

Parent links verified via `$expand=relations` after creation — each child's
`System.LinkTypes.Hierarchy-Reverse` relation points at the correct parent id. Learned from the
earlier wiki-content mistake this session (trusting a 200 response without checking the actual
saved content) — verified this one properly rather than assuming success from the create response
alone.

## Explicitly out of scope (not built)
- Doctor/Patient record management screens — the actual "admin management" capability the user
  described as the end goal. Not scoped yet (what can an Admin do to a Doctor/Patient record?
  view-only vs. edit vs. delete? undefined). Separate future story once defined.
- Real backend wiring — blocked on an `Admin` role + admin-management endpoints being designed in
  `HC.AI.Identity.Api` (or wherever Architect decides admin operations should live).

## References
- [TASK015](../../scrum/tasks/TASK015_US014_US015_admin_identity_mock.md)
- [US014](../../product_owner/user_stories/US014_admin_login.md) /
  [US015](../../product_owner/user_stories/US015_admin_signup.md)
- [BACKLOG.md](../../product_owner/backlog/BACKLOG.md) — PB031
