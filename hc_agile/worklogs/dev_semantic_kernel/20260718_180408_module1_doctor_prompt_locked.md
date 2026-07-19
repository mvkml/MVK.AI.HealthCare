# Module 1 (Locked): Doctor Persona — Natural-Language Prompt Assistant, End-to-End

**Status:** Locked — proven end-to-end, verified live.
**Owns:** Dev Semantic Kernel Agent
**Related:** US007, TASK006, TASK008, ADR001

## What this module does (client-facing summary)

A Doctor sends a natural-language message, plus optional generation controls (how long the
answer should be, how creative vs. precise it should sound), to a single REST endpoint. The
system validates the request, decides which AI model should answer, runs the request through
that model, and returns a clean, structured response — including whether it succeeded, what
model answered, and how long it took. The AI provider behind the scenes (currently a local model,
Ollama) is swappable without changing anything the Doctor-facing endpoint exposes.

This is the first feature built completely from the API entry point down to the AI model and
back — every other persona (Patient, Provider, Hospital, etc.) will follow the same shape.

## Entry point

`POST /api/Doctor/provide-prompt`

Request:
```json
{
  "message": "hi",
  "maxTokens": 200,
  "temperature": 0.3,
  "topP": 0.9,
  "topK": 40,
  "frequencyPenalty": 0,
  "presencePenalty": 0,
  "stopSequences": [],
  "stream": false,
  "seed": null
}
```

Response (verified live, 2026-07-18):
```json
{
  "content": "Hello! How can I assist you today?",
  "finishReason": "",
  "promptTokens": 0,
  "completionTokens": 0,
  "totalTokens": 0,
  "modelUsed": "qwen2.5:7b",
  "latencyMs": 30908,
  "isSuccess": true,
  "errorMessage": ""
}
```
(`finishReason`/token counts are blank/0 — documented gap, the Ollama connector doesn't expose
them, not a defect.)

## Step-by-step flow

| # | Step | Component | What happens |
|---|---|---|---|
| 1 | Request arrives | `DoctorController.ProvidePrompt` | Receives the raw `PromptRequest` |
| 2 | Validate | `PromptValidationUtility` | Rejects with a 400 + reasons if `message` is empty, or `temperature`/`topP`/`topK`/`maxTokens` are out of range |
| 3 | Map request → item | `DoctorPromptMapper.ToPromptItem` | Copies the request's fields onto a `PromptItem` — the self-contained unit every layer below uses from here on |
| 4 | Decide the model | `LLMModelBL.GetModelDetails` | Resolves which LLM/provider answers this request (today: always Ollama, from config) — the one place this decision is made |
| 5 | Orchestrate | `DoctorSemanticProcess.ProcessAsync` | Builds the Doctor's system prompt (via `DoctorPromptProvider`), times the call, hands off to the execution layer |
| 6 | Execute | `SemanticProcessService.ExecutePromptAsync` | Builds a Semantic Kernel `Kernel` for the resolved provider (`KernalFactory`), runs the prompt through it |
| 7 | Map result → response | `PromptItemMapper.ToPromptResponse` / `ToErrorPromptResponse` | Turns the raw model output (or an exception) into the structured `PromptResponse` |
| 8 | Return | `DoctorController` | Sends the `PromptResponse` back as 200, or the validation errors as 400 |

Every layer from step 3 onward passes one shared object (`PromptModel`, per ADR001) instead of
separate loose parameters — so adding a new field later touches only the layer that needs it, not
every function signature in the chain.

## What this module does NOT cover yet (so client expectations are set correctly)

- Only one LLM provider (Ollama) is wired — provider selection per persona/user-type is backlog
  item PB019, not yet built.
- Only the Doctor persona is implemented this way; Patient/Provider/Hospital/etc. controllers are
  still empty stubs.
- No guardrail/restriction-template enforcement yet (Guardrail Layer exists as a project, not
  wired in).
- No database-backed prompt source yet — the Doctor's system prompt is a hardcoded string in
  `DoctorPromptProvider`.

## Verification

Full solution builds clean (0 errors, 2 pre-existing unrelated nullable warnings in
`BaseModel.cs`). Endpoint called live against a running local Ollama instance, returned 200 with
a real model-generated response, as shown above.
