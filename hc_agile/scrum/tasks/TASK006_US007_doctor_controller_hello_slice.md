# TASK006 - Doctor controller "hi" end-to-end slice

**US:** US007
**Status:** Done — verified live 2026-07-17/18. Superseded/extended by TASK008; see
`hc_agile/worklogs/dev_semantic_kernel/20260718_180408_module1_doctor_prompt_locked.md` for the
locked end-to-end module writeup.

## Description
Wire `UCDoctorController` (currently an empty stub) through the Agent Layer down to the Business
Layer so that a basic greeting ("hi") gets a correct, real response — proving the full request
path for the Doctor persona, the same way `HelloWorldController -> HC.AI.MAPI.BL` already proved
the plain Controller -> BL path.

This is the first, smallest possible vertical slice for US007 — no real query-building, Ollama
call, or Guardrail check yet. Just: Doctor persona hits the API, gets a correct basic response,
end to end.
