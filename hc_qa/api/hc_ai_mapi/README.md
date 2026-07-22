# QA — HC.AI.MAPI

Playwright API test suite for `HC.AI.MAPI` (Dev Semantic Kernel Agent's Ollama/Semantic Kernel
backend). Sibling to `hc_qa/api/ai_hc_api/` and `hc_qa/api/hc_ai_identity_api/`.

**Status:** 8/8 passing, executed against the live `HC.AI.MAPI` (`localhost:5150`) + Ollama
(`localhost:11434`), real generation (not mocked).

Covers the persona -> model routing added on top of `POST /api/Doctor/provide-prompt`
(`DoctorPromptMapper` -> `LLMModelBL` -> `LLMOptionsFactory`, reading the `HCDocExecutor` /
`HCPatientExecutor` sections of `appsettings.json`). `PromptResponse.ModelUsed` echoes the Ollama
model tag that actually served the request, so the suite asserts routing end-to-end via the API
response rather than by inspecting Ollama logs.

## Layout
```
hc_ai_mapi/
├── tests/
│   └── provide-prompt.spec.ts   ← persona routing (5) + validation/shape (3)
└── fixtures/
    └── prompt.ts                ← buildPromptRequest()
```

## Setup dependencies
- `HC.AI.MAPI` running on `http://localhost:5150`
- Ollama running on `http://localhost:11434` with `hc-doctor-executor:latest` and
  `hc-patient-executor:1.1` pulled (`ollama list` to check)
- `workers: 1` in `playwright.config.ts` — each case is a real ~7B-model generation; running
  more than one at a time against a single Ollama instance just queues them anyway

## Notes
- The `PersonaModelResolutionController` / `BL/Persona/*` files added alongside this change are
  an unrelated, unwired mock (EPIC001/PB032/TASK017 — see the comment in `Program.cs`), not part
  of the live `provide-prompt` path. Not covered here; will get its own suite when it's wired in.
- "Unrecognized persona falls back to the doctor executor" is today's actual behavior
  (`DoctorPromptMapper` only special-cases an exact, case-insensitive match on `"Patient"`) — the
  test documents it as-is, not as a defect.
