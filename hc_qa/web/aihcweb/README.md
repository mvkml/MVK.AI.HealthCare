# QA — aihcweb

Playwright end-to-end UI test suite for `hc_ui/aihcweb` (Dev Angular Agent's chat web app).
Sibling to `hc_qa/web/mvkhcapp/`.

**Status:** Test specs organized by user type — `admin-user/` (mock-only) and `site-user/`
(real-backend, split into `dr-user/`/`patient-user/` ahead of any future validation divergence,
even though `/login` and `/signup` are one shared page today). `demos/demo1` and `demo2` were
written against the old mock (`AuthMockService`); the app now calls the real
`HC.AI.Identity.Api` backend (`hc_ui/aihcweb/src/app/features/auth/data/auth.service.ts`), so the
demos' hardcoded `doctor@demo.health` / `patient@demo.health` accounts are unverified against the
real DB — flag before relying on them again.

## Layout
```
aihcweb/
├── demos/                        ← standalone setup scripts (npm run demo:<name>) — see demos/README.md
├── tests/
│   ├── admin-user/                 ← Admin persona (AdminAuthMockService, mock-only)
│   │   ├── admin-login/admin-login.spec.ts
│   │   └── admin-signup/admin-signup.spec.ts
│   └── site-user/                  ← Doctor/Patient (real HC.AI.Identity.Api backend)
│       ├── dr-user/
│       │   ├── login/login.spec.ts
│       │   └── signup/signup.spec.ts
│       └── patient-user/
│           ├── login/login.spec.ts
│           └── signup/signup.spec.ts
└── fixtures/
    ├── auth.ts             ← buildTestUser() + seedUser() (real API, site-user)
    └── admin-auth.ts       ← buildAdminTestUser() + seedAdminAccount() (localStorage, admin-user)
```

## Setup
```
cd hc_qa/web/aihcweb
npm install
```
Requires `hc_ui/aihcweb` running on `http://localhost:4200` (`npm start`) and
`HC.AI.Identity.Api` running on `http://localhost:5008`
(`hc_apis/az/hc_core_apis/HC.AI.Identity.Api/HC.AI.Identity.Api && dotnet run --urls http://localhost:5008`).
