# US009 - Authentication: Login, Sign Up, Home (Angular, mock-first)

**As a** Doctor or Patient
**I want to** log in / sign up and land on a persona-aware home page
**So that** I have a real entry point into the app instead of landing directly on the chat screen

## Background
Reference implementation exists in the sibling `ai_hr` project — `hr_ui/aihrweb` (Angular:
Login/Signup/Forgot-Password/Dashboard pages) and `AI.HR.Api` (.NET: JWT-based
`UsersController` — signup/login/forgot-password/reset-password/roles, EF migrations for a
Users+Roles schema). Full path inventory recorded in the Dev Angular worklog for this story.

Per user instruction: **structure can be reused as reference, theme should be changed** to match
this project's existing clinical design system (`hc_ui/aihcweb/src/styles.css` tokens), not copied
verbatim — the HR reference is marketing-styled (social login buttons, gradient hero) and
domain-specific (HR copy), neither of which fits a healthcare tool.

## Backend status
**Real now.** `HC.AI.Identity.Api` (`hc_ai_in/mapi/HC.AI.Identity.Api`) provides live
signup/login (JWT-issuing), merged into `AI_HealthCarePatient`. Wired up in
[TASK013](../../scrum/tasks/TASK013_US009_wire_real_auth_backend.md) — see that worklog for the
full detail, including a naming discrepancy that had to be resolved first (docs said `AI.HR.Api`,
the actual project is `HC.AI.Identity.Api`).

## Acceptance Criteria
- [x] Login page — email/password, loading + error states, **real HTTP call** to
      `POST /api/users/login`
- [x] Sign-up page — full name, email, persona selection (Doctor / Patient), password +
      confirmation with validation, **real HTTP call** to `POST /api/users/signup`
- [x] Home page — persona-aware post-login landing: Doctor sees a link into the existing chat
      feature; Patient sees a link into `/patient-chat` (US010)
- [x] Route guard — `/home` and `/chat` require a real logged-in session; unauthenticated visitors
      are redirected to `/login`
- [x] Chat page's persona display reflects the actual logged-in user, not a separate hardcoded
      value (avoids two different "who's logged in" sources existing in the same app)
- [x] Unit tests for the new components/service — 40 tests total in the app (up from 9), all
      HTTP-mocked against the real contract rather than an in-memory demo list

## Explicitly out of scope for this story
- Forgot/reset password pages — the backend supports them, the UI doesn't have them yet
  (flagged in TASK013 as a follow-up decision, not silently dropped)
- Token storage strategy (`localStorage` vs httpOnly cookie) — still not formally signed off;
  `localStorage` is the pragmatic default forced by the API returning the JWT in the response body
- Multi-page HR-style dashboard shell (upload/employees/document-intelligence) — not relevant to
  this project's scope

## Priority: High
## Status: Done — real backend wired
## Sprint: Unscheduled — not yet assigned to a sprint plan
## Worklogs:
- [20260719_190253_mock_auth_login_signup_home.md](../../worklogs/dev_angular/20260719_190253_mock_auth_login_signup_home.md) — initial mock-first UI build
- [20260719_223524_wire_real_identity_api.md](../../worklogs/dev_angular/20260719_223524_wire_real_identity_api.md) — real backend wiring
