# Demo 1 — Doctor Persona: Natural-Language Prompt Assistant, End-to-End

**Status:** Locked, verified live (backend + Angular UI both complete).

## What to show

A Doctor types a message into a chat window (`hc_ui/aihcweb`). It travels through a validated
REST API, gets answered by a locally-running AI model (Ollama), and the response renders back in
the chat UI — proving the full stack works end to end: **UI → API → validation → business logic
→ AI model → back to UI.**

**Important framing for the client:** this module proves the AI-calling plumbing works safely and
end-to-end. It does **not** yet answer real patient-data questions ("recent patients", "details
for John Smith") — that's the next module (backlog PB020), which adds database-grounded lookups
through a validated query layer. Be explicit about this boundary if asked.

## Pre-Demo Checklist

- [ ] Ollama running locally (`http://localhost:11434`)
- [ ] `qwen2.5:7b` model pulled and available (`ollama list` shows it)
- [ ] Backend running: `http://localhost:5150` (`dotnet run` from `HC.AI.MAPI/HC.AI.MAPI`)
- [ ] Angular UI running: `http://localhost:4200` (`npm start` from `hc_ui/aihcweb`)
- [ ] Sent one test message through the UI just before presenting and got a real response back
      (don't rely on it having worked earlier in the day — restart state can change)
- [ ] Ready to state the boundary if asked: this module doesn't answer real patient-data
      questions yet (no database lookups) — that's backlog PB020, not built yet

## How to run it live

1. **Ollama must be running locally first** — this is the actual AI model, and nothing answers
   without it:
   - Ollama itself running (`http://localhost:11434` — check with `ollama list` or
     `curl http://localhost:11434`)
   - The model pulled: `qwen2.5:7b` (`ollama pull qwen2.5:7b` if not already present)
   - If Ollama isn't running, the demo fails at step 4 below with a connection error from the API
     (a 200-with-`isSuccess:false` or a 500, not a clean "down" message) — start Ollama and retest
     before presenting
2. Start the backend: `cd hc_ai_in/mapi/HC.AI.MAPI/HC.AI.MAPI && dotnet run --urls http://localhost:5150`
3. Start the UI: `cd hc_ui/aihcweb && npm start` (serves on `http://localhost:4200`)
4. Open `http://localhost:4200`, type a message (e.g. "hi"), send — response comes back through
   the real Ollama-backed endpoint. Expect ~30s latency locally — don't rush past this live, it's
   normal for this model/hardware, not a hang.
5. (Optional, to show the raw API) Swagger UI at `http://localhost:5150/swagger/index.html`, or
   `POST http://localhost:5150/api/Doctor/provide-prompt` directly.

## The flow (plain-language version for walking a client through it)

| Step | What happens |
|---|---|
| 1 | Doctor types a message in the chat UI and hits send |
| 2 | The API validates the request (e.g. rejects an empty message) |
| 3 | The system decides which AI model should answer (currently a local model, swappable later) |
| 4 | The request is built into a prompt and sent to the AI model |
| 5 | The AI's answer comes back, gets cleaned up into a consistent response shape |
| 6 | The chat UI displays the answer as the assistant's reply |

## Source of truth (don't duplicate — link back to these)

This file is a demo-day summary only. For the real, maintained detail, see:

- **Full technical writeup:** [`hc_agile/worklogs/dev_semantic_kernel/20260718_180408_module1_doctor_prompt_locked.md`](../../../hc_agile/worklogs/dev_semantic_kernel/20260718_180408_module1_doctor_prompt_locked.md)
- **Backend user story:** [`hc_agile/product_owner/user_stories/US007_healthcare_ai_assistant.md`](../../../hc_agile/product_owner/user_stories/US007_healthcare_ai_assistant.md)
- **Frontend user story (now fully checked off):** [`hc_agile/product_owner/user_stories/US008_chat_ui_doctor_persona.md`](../../../hc_agile/product_owner/user_stories/US008_chat_ui_doctor_persona.md)
- **Architecture decision (Context Object pattern):** [`hc_agile/architecture/decisions/ADR001_prompt_model_context_object.md`](../../../hc_agile/architecture/decisions/ADR001_prompt_model_context_object.md)
- **What's next (database-grounded answers, still open):** `hc_agile/product_owner/backlog/BACKLOG.md` — PB020
- **All live URLs:** [`hc_agile/REFERENCE_LINKS.md`](../../../hc_agile/REFERENCE_LINKS.md)
