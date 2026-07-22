# Wire Angular auth to the real HC.AI.Identity.Api (2026-07-19)

Implements [TASK013](../../scrum/tasks/TASK013_US009_wire_real_auth_backend.md) / PB025.

## Discrepancy found and resolved before writing any code
TASK013 and the Dev .NET/SQL worklogs consistently describe the backend as `AI.HR.Api`
(`AiHrDbContext`, `AI.HR.Api.sln`). The actual folder on disk is
`hc_ai_in/mapi/HC.AI.Identity.Api`, fully renamed (`HC.AI.Identity.*` namespaces throughout,
confirmed by reading `Program.cs`/`UsersController.cs`/the `Models` directly rather than trusting
the worklog text). Docs appear to have gone stale after a rename that happened later the same
day. Proceeded from the real code, not the worklog's class/project names.

## Also found: CORS configured for the wrong port
`Program.cs` has `policy.WithOrigins("http://localhost:4201")` — this project's Angular dev server
actually runs on **4200** (established since US008). Didn't touch the .NET CORS config (same
reasoning as the `HC.AI.MAPI` integration): proxying through the Angular dev server means the
browser never makes a cross-origin request, so the CORS mismatch doesn't block anything, but it's
worth Dev .NET fixing or generalizing if the API is ever hit directly from a browser.

## What was built
- `proxy.conf.json` — added `/api/users -> http://localhost:5008` (more specific, listed first)
  alongside the existing `/api -> http://localhost:5150` (`HC.AI.MAPI`) — no path collision, since
  Identity's routes are `/api/users/*` and MAPI's are `/api/Doctor/*` etc.
- `AuthMockService` renamed to `AuthService` (the "Mock" name was no longer accurate) and
  rewritten against real `HttpClient` calls to `POST /api/users/login` / `POST /api/users/signup`,
  keeping the exact same public surface (`currentUser` signal, `isLoggedIn` computed, `login()`,
  `signUp()`, `logout()`) — every consumer (`Login`, `Signup`, `Home`, `ChatPage`,
  `PatientChatPage`, `auth.guard.ts`) needed only an import rename, zero logic changes
- Role mapping: `RoleId 1 = Doctor`, `RoleId 2 = Patient` (confirmed live via
  `GET /api/users/roles`) — matches what Dev .NET's roles-simplification worklog said, verified
  directly rather than assumed
- Session storage: JWT + user now both persisted to `localStorage` under `hc_auth_session`
  (previously just a plain mock user object). **Not a final decision** — the API returns the token
  in the response body, not a `Set-Cookie`, so `localStorage` is the only option that works
  without a backend change. The httpOnly-cookie question TASK013 flagged for Architect/PO sign-off
  is still open; this is the pragmatic default the API's current shape forces, not a considered
  answer to that question
- Signup's `company` field: backend requires it non-empty (`UserValidationService`: "Company is
  required") but it's a meaningless leftover from the HR-domain schema for a Doctor/Patient
  signup. Sent as a fixed placeholder (`"N/A"`) rather than adding a UI field for it — flagged, not
  silently designed around
- Login page's old "Demo accounts: doctor@demo.health / patient@demo.health" hint removed — those
  accounts only ever existed in the client-side mock; the real seeded users are the seed rows
  copied over from `AI_HR` (`Test User`, etc.), whose passwords aren't known
- **Left out of this pass** (per TASK013's own flagged optionality, not silently dropped):
  forgot-password / reset-password pages. The backend supports them; the UI doesn't have them yet

## Verification
- `ng build` — clean
- `ng test` — 40/40 passing (up from 36) — `auth.service.spec.ts` fully rewritten with
  `HttpTestingController` (login success/401/network-failure, signup success/validation-error,
  RoleId↔persona mapping both directions, localStorage session restore on a fresh injector);
  `login.spec.ts`/`signup.spec.ts` now mock the real HTTP calls instead of relying on demo
  accounts; `chat-page.spec.ts`/`home.spec.ts`/`patient-chat-page.spec.ts`/`auth.guard.spec.ts`
  simplified to set `auth.currentUser` directly rather than round-tripping a fake login, since
  their concern is "given a logged-in user, does X work," not auth mechanics
- **Live, real backend, through the actual proxy path** (not just HTTP-mocked tests):
  - `GET http://localhost:4200/api/users/roles` → confirmed `{1: Doctor, 2: Patient}` through the
    proxy, proving the multi-target proxy routes `/api/users/*` to :5008 and leaves `/api/Doctor/*`
    going to :5150 (`HC.AI.MAPI`), both correctly
  - `POST http://localhost:4200/api/users/signup` then `POST .../login` with a throwaway account
    (`claude.verification.test@example.com`) — signup succeeded (userId 4014), login returned a
    real signed JWT with `roleId: 1` matching what was sent at signup

## ⚠️ Live data created, not cleaned up
The verification signup above created a real row in `AI_HealthCarePatient.Users` (userId 4014,
email `claude.verification.test@example.com`). No delete-user endpoint exists on
`HC.AI.Identity.Api`, and this session has no direct DB access tool to remove it the way Dev .NET
manually deleted their own throwaway test row. **Flagging for Dev .NET/SQL to clean up** —
`DELETE FROM Users WHERE Email = 'claude.verification.test@example.com'`.

## References
- [TASK013](../../scrum/tasks/TASK013_US009_wire_real_auth_backend.md)
- [BACKLOG.md](../../product_owner/backlog/BACKLOG.md) — PB025
- [Dev .NET worklog](../dev_dotnet/20260719_200000_auth_db_merge_ai_hr_api.md) / [roles simplification](../dev_dotnet/20260719_204500_simplify_roles_doctor_patient.md)
