# Demo Deck — Part 1: AI/ML Foundations → Ollama → Why This Model

**Purpose of this file**: source content for a PPT deck, to be handed to another AI session
for slide generation. Audience is **mixed**: some client-side attendees have no ML background,
others are practicing data scientists / ML engineers. Content below is layered so each topic
opens with a plain-language framing, then a technical layer underneath — the PPT builder should
put the plain-language line on the main slide and the technical layer as sub-bullets or speaker
notes, not cut either one.

**Audience-balance rule for the PPT builder**: don't dumb down and don't over-index technical
either. Structure: one confident, correct sentence a non-technical exec can follow, immediately
followed by the precise technical detail a data scientist would want to see, so neither audience
feels talked past.

---

## 1. What is Artificial Intelligence (AI)

**Plain**: AI is software that performs tasks which normally require human judgment —
recognizing patterns, making decisions, understanding language — without being explicitly
programmed with a rule for every case.

**Technical**: AI is the umbrella field. It includes rule-based/expert systems (hand-coded
logic), search and planning, and — the branch relevant here — Machine Learning, where the
"rules" are learned from data rather than written by a programmer.

## 2. What is Machine Learning (ML)

**Plain**: Instead of a developer writing "if X then Y" for every possible case, ML shows the
system thousands (or billions) of examples, and the system learns the pattern itself.

**Technical**: ML is a function-approximation problem — given inputs and (usually) known correct
outputs, the model adjusts internal parameters (weights) to minimize prediction error. Broad
categories: supervised (labeled data), unsupervised (pattern discovery, no labels), and
reinforcement learning (learn from reward signal). The LLM layer discussed later is trained
primarily via self-supervised learning on text, then refined with human feedback.

## 3. What is a (Deep) Neural Network

**Plain**: A neural network is a layered system of simple math units, loosely inspired by
neurons in the brain, that combine to recognize complex patterns — the more layers, the more
abstract the patterns it can learn. "Deep" just means "many layers."

**Technical**: Each layer applies a weighted sum + non-linear activation function to its input
and passes the result to the next layer. Depth lets the network compose simple features into
complex ones (e.g. in language: characters → words → phrases → meaning). Modern LLMs use a
specific neural network architecture called the **Transformer**, which uses "attention" to
weigh how much every word in a sentence should influence every other word — this is what lets
the model handle long-range context and produce coherent, relevant language.

## 4. What is a Large Language Model (LLM)

**Plain**: An LLM is a deep neural network trained on massive amounts of text, whose core skill
is predicting "what word comes next" — and it turns out that skill, at large enough scale,
produces something that can hold conversations, answer questions, write code, and follow
instructions.

**Technical**: LLMs are Transformer-based models with billions of parameters, trained in stages:
(1) pretraining — next-token prediction over a huge text corpus, (2) instruction
tuning/fine-tuning — teaching it to follow prompts rather than just autocomplete, (3) alignment
(e.g. RLHF) — shaping tone, safety, refusal behavior. "Parameter count" (7B, 8B, 70B, etc.) is a
rough proxy for model capacity — more parameters generally means more nuanced reasoning, at the
cost of more compute/memory to run.

---

## 5. What is Ollama

**Plain**: Ollama is a tool that lets you run these LLMs directly on your own computer or
server — not through a cloud API like ChatGPT or Claude — so the model, and your data, never
leave your infrastructure.

**Technical**: Ollama is an open-source local LLM runtime. It packages model weights,
tokenizer, and a serving layer behind a simple local REST API (`http://localhost:11434` by
default — `/api/chat`, `/api/generate`, `/api/tags`). It manages model downloads/versions/
quantization and exposes tool-calling (function-calling) for supported models, which is the
mechanism this project's architecture relies on.

## 6. History of Ollama

- Released in **2023** as an open-source project, initially macOS-first, later adding
  Linux and Windows support.
- Built on top of the broader open-weight-LLM ecosystem that took off after Meta released
  LLaMA's weights (2023) — Ollama's contribution was making locally-run inference *simple*:
  one command to pull a model, one command to run it, instead of manually managing inference
  engines and model file formats.
- Positioned itself as "the Docker of LLMs" — a consistent, scriptable way to package and run
  models locally, with a public model library (`ollama.com/library`) analogous to Docker Hub.
  Under the hood it builds on the `llama.cpp` inference engine for efficient CPU/GPU execution.
- Since then has grown into the de facto standard for local/on-prem LLM serving, with broad
  ecosystem support (LangChain, Semantic Kernel, and most agent frameworks ship an Ollama
  connector) and now first-class tool-calling support, which is what made this project's
  architecture (Ollama → MCP tool → SQL Server) feasible without a cloud LLM dependency.

## 7. Advantages of Ollama (why it fits this project specifically)

| Advantage | Why it matters for a healthcare client |
|---|---|
| **Data privacy** | Patient data (PHI) and every question asked about it never leaves the client's own infrastructure — no third-party API sees it. Directly relevant to HIPAA-style compliance concerns. |
| **Cost** | No per-token/per-request API billing. Cost is hardware only, already sunk or predictable, vs. an open-ended cloud LLM bill that scales with usage. |
| **Offline / on-prem capable** | Works with no internet dependency — relevant for hospital environments with strict network policies. |
| **Control over model choice/versioning** | The client isn't dependent on a vendor's model deprecation schedule — models are pulled once and run indefinitely, versioned explicitly. |
| **Open ecosystem** | Not locked into one vendor's tooling — same architecture could later swap in a different local model with minimal rework. |

## 8. Why this specific model — `qwen2.5:7b`

**Plain**: This project's core job for the LLM isn't "have a great conversation" — it's
"reliably turn a doctor's question into a precise, structured request." We picked the model
that's specifically strong at *that* skill, not the biggest or most famous one.

**Technical**: The task is structured-output generation — converting natural language into a
constrained JSON query object matching our query DSL (§4 of the technical design doc). Qwen2.5
was explicitly trained and benchmarked for function-calling/structured-output accuracy (e.g.
Berkeley Function-Calling Leaderboard performance), which is a more targeted fit than a
general-purpose chat model of similar size (e.g. Llama 3.1:8b, which is capable but not
specifically optimized for this). At 7B parameters (~4.7 GB), it's lightweight enough to run
comfortably without enterprise-grade GPU hardware, while still reliably producing valid JSON —
the property the entire safety architecture (query validated against a schema allow-list before
touching the database) depends on.

Medically-fine-tuned models (e.g. Meditron, BioMistral) were explicitly considered and rejected
— this architecture grounds every answer in real SQL data returned by a tool call, so the model
never needs to "know medicine" itself. It only needs to understand the question, generate a
correct structured query, and phrase the result back in plain language — medical fine-tuning
optimizes for a skill this design deliberately doesn't rely on.

**Upgrade path (transparency for the client)**: `qwen2.5:7b` is the Phase 1 starting model.
`command-r` (Cohere, 35B) is the identified later-phase upgrade once the core architecture is
proven — purpose-built for enterprise RAG/tool-use workflows, at a heavier compute cost. This is
worth stating in the demo: the model choice was deliberate and phased, not a one-time pick.

---

## Suggested slide breakdown (for the PPT builder)

| Slide | Content | Audience note |
|---|---|---|
| 1 | Title / agenda | — |
| 2 | What is AI → ML → Neural Networks (one slide, funnel diagram) | Keep to the "Plain" lines; technical layer as speaker notes only |
| 3 | What is an LLM | Plain line as headline, Transformer/attention as one sub-bullet for the technical audience |
| 4 | What is Ollama | Plain definition + the local-vs-cloud diagram already in the technical design doc |
| 5 | Brief history of Ollama | Light-touch, 3-4 bullets max — context, not the focus of the demo |
| 6 | Why Ollama (advantages table) | Full table — this is the slide the client's decision-makers will remember |
| 7 | Why `qwen2.5:7b` specifically | Table comparing vs. Llama 3.1 (already in the design doc) — this is the slide the data scientists in the room will scrutinize most; don't compress it |
| 8 | Roadmap teaser (Phase 1 → command-r → Azure) | Sets up the next MD file / next part of the deck |

## Status

Drafted, not yet locked. Awaiting your review before you generate the PPT. Next MD files (not
yet written) should cover: the architecture/flow diagram (MCP + query DSL + grounding
guardrail), and the live demo script itself.
