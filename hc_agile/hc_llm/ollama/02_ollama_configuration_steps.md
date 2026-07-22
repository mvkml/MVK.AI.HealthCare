# Demo Deck — Part 2: Ollama Configuration — Step by Step (Technical)

**Purpose of this file**: source content for the PPT (technical/backup slides) and for anyone
reproducing the setup. Companion to [`01_intro_ai_ml_ollama.md`](01_intro_ai_ml_ollama.md) —
that file explains *what* Ollama is and *why* it was chosen; this file is *how it's actually
configured* in this project, traced from the real code, not a generic tutorial.

**Audience note for the PPT builder**: this is the slide (or appendix) the client's data
scientists/engineers will read closely. Keep every step accurate to the actual project files
referenced below — don't generalize or simplify away real file/config names.

---

## Step 1 — Install Ollama

- Download and install from the official Ollama site for the target OS (Windows/macOS/Linux).
- Installs as a background service and exposes a local REST API once running.

## Step 2 — Verify the service is running

Base URL is `http://localhost:11434` by default (configurable — see Step 6).

```
curl http://localhost:11434/api/version
```

A JSON response with a `version` field confirms Ollama is up.

## Step 3 — Pull the model

This project's locked model choice (see Part 1, §8) is `qwen2.5:7b`:

```
ollama pull qwen2.5:7b
```

## Step 4 — Confirm the model is available locally

```
ollama list
```

Expect `qwen2.5:7b` in the output with its size and pull date. The application will fail at
request time (not at startup) if the configured model name isn't pulled yet.

## Step 5 — Smoke-test the model directly (before wiring the app)

```
curl http://localhost:11434/api/chat -d "{\"model\":\"qwen2.5:7b\",\"messages\":[{\"role\":\"user\",\"content\":\"say hi\"}],\"stream\":false}"
```

Confirms Ollama + the model respond correctly, independent of the .NET application — isolates
"is Ollama the problem" from "is the app the problem" during troubleshooting.

## Step 6 — Configure the application to point at Ollama

The backend (`HC.AI.MAPI`) reads Ollama configuration from `appsettings.json`, via a dedicated
LLM layer (`HC.AI.MAPI.Llm`) that owns the model/endpoint settings.

**File**: [`hc_ai_in/mapi/HC.AI.MAPI/HC.AI.MAPI/appsettings.json`](../../../hc_ai_in/mapi/HC.AI.MAPI/HC.AI.MAPI/appsettings.json)

```json
{
  "Ollama": {
    "Provider": "Ollama",
    "BaseUrl": "http://localhost:11434",
    "Model": "qwen2.5:7b"
  }
}
```

| Key | Purpose |
|---|---|
| `Provider` | Labels which LLM provider is active — kept explicit so a future swap (e.g. to a cloud provider) is a config change, not a code change |
| `BaseUrl` | Where the app sends requests — points at the local Ollama REST API |
| `Model` | Which pulled model to call — must match an entry in `ollama list` exactly, including the tag (`:7b`) |

This maps directly to the strongly-typed options class:

**File**: [`hc_ai_in/mapi/HC.AI.MAPI/HC.AI.MAPI.Llm/OllamaOptions.cs`](../../../hc_ai_in/mapi/HC.AI.MAPI/HC.AI.MAPI.Llm/OllamaOptions.cs)

```csharp
public class OllamaOptions
{
    public const string SectionName = "Ollama";
    public string Provider { get; set; } = "Ollama";
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "qwen2.5:7b";
}
```

## Step 7 — How the app wires it up at startup

**File**: [`hc_ai_in/mapi/HC.AI.MAPI/HC.AI.MAPI/Program.cs`](../../../hc_ai_in/mapi/HC.AI.MAPI/HC.AI.MAPI/Program.cs)

```csharp
builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection(OllamaOptions.SectionName));
builder.Services.AddHttpClient<IOllamaClient, OllamaClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});
```

- `appsettings.json`'s `Ollama` section binds to `OllamaOptions` via the standard .NET Options
  pattern.
- A typed `HttpClient` (`IOllamaClient` / `OllamaClient`) is registered with its `BaseAddress`
  set from that config — every call this client makes automatically targets the configured
  Ollama instance, no hardcoded URL in the calling code.
- `IOllamaClient` is what the rest of the app (`HC.AI.MAPI.SemanticProcess`, `HC.AI.MAPI.AL`)
  depends on — swapping providers later means changing config + this one client, not every
  caller.

## Step 8 — Changing the model or endpoint later

Two supported ways, in order of preference:

1. **`appsettings.json`** — edit the `Ollama` section directly for a persistent change.
2. **Environment variable override** — ASP.NET Core config supports env-var overrides of any
   `appsettings.json` key using double underscores, e.g.:
   ```
   Ollama__Model=llama3.1:8b
   Ollama__BaseUrl=http://localhost:11434
   ```
   Useful for demo environments or CI without editing the checked-in file.

No code change is required for either — this was a deliberate design choice (config-driven
provider/model, per Step 6-7) so the model shortlist evaluated in Part 1 can be swapped without
touching `HC.AI.MAPI.Llm` internals.

## Step 9 — Optional: bake a guardrail system prompt into the model (Modelfile)

For a system-level guardrail that doesn't depend on the calling code sending it every request,
Ollama supports a `Modelfile` (like a Dockerfile, but for models):

```
FROM qwen2.5:7b

SYSTEM """
You are a clinical data assistant. Only answer using data returned by tool calls.
Never state a fact not present in a tool result. If a tool returns no rows, say so explicitly.
"""

PARAMETER temperature 0.1
```

Build and use it:

```
ollama create healthcare-doctor -f Modelfile
ollama show healthcare-doctor --modelfile   # verify what's baked in
```

Then point `Ollama:Model` in `appsettings.json` at `healthcare-doctor` instead of the base
model. **Not yet adopted in this project** — current grounding guardrail is applied in the
prompt layer (`HC.AI.MAPI.Prompt`) per-request rather than baked into a custom model. Flagged
here as a documented option, not a locked decision.

## Step 10 — End-to-end verification checklist

Matches the pre-demo checklist already locked in
[`001demo/README.md`](../../../hc_demo/hc_demo_main/001demo/README.md):

- [ ] `ollama list` shows `qwen2.5:7b`
- [ ] `curl http://localhost:11434/api/version` responds
- [ ] Backend running (`dotnet run` from `HC.AI.MAPI/HC.AI.MAPI`) and reads config without error
- [ ] A real request through the app (UI or Swagger) returns a live Ollama-generated answer, not
      a config/connection error

---

## Suggested slide breakdown (for the PPT builder)

| Slide | Content | Audience note |
|---|---|---|
| 1 | Install → pull → verify (Steps 1-4) | Fast, 1 slide — this is setup, not the interesting part |
| 2 | App configuration: `appsettings.json` → `OllamaOptions` → DI-registered `HttpClient` (Steps 6-7) | The slide the engineers/data scientists will care about most — show the real code, not a diagram substitute |
| 3 | Swappability: env var override + config-driven provider (Step 8) | Speaks to "are we locked into this model/vendor" — a likely client question |
| 4 (appendix/backup) | Modelfile guardrail option (Step 9) | Only if asked "how do you keep it from hallucinating at the model level" — otherwise skip live, keep as backup slide |

## Status

Drafted, not yet locked. Traced directly from the current `HC.AI.MAPI.Llm` / `appsettings.json`
source — re-verify against the code if this file is reused after further backend changes.
