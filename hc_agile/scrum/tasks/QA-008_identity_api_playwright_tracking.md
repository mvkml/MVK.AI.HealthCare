# QA-008 - Track hc_ai_identity_api Playwright suite (backfill)

**US:** US022
**Status:** Closed — Executed, Passed (19/19)

## Description
`hc_qa/api/hc_ai_identity_api/tests/users.spec.ts` (19 tests against `HC.AI.Identity.Api`) was
written and executed earlier this session but never got its own QA-xxx task file — tracked
retroactively here as part of the coverage audit (QA-006).

## Coverage
19 tests: Roles (1), Signup (6: valid + default-role + 4 validation errors), Login (5: valid token
+ wrong password + unknown email + bad format + missing password), Forgot Password (3), Reset
Password (4).

## Result
Already executed earlier this session — 19/19 passing. See conversation history / `hc_qa/api/hc_ai_identity_api/README.md`.

## References
- [US022](../../product_owner/user_stories/US022_qa_test_coverage_inventory.md)
