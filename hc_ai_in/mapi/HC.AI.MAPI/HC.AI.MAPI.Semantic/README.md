# HC.AI.MAPI.Semantic

## Layer
Semantic Process Layer — owns Kernel creation via a Factory pattern (`Factory/`). The caller
(a higher layer, e.g. Prompt Layer or Agent Layer) supplies which `provider` to use;
`SemanticKernelFactory` builds the matching Kernel. Only `"Ollama"` is implemented today — any
other provider value throws `NotSupportedException` until it's added. Designed so multiple
providers (OpenAI, Azure OpenAI, etc.) can be added later without changing callers.

## Author / Architect / Designer
Vishnu Kiran M

Azure AI Solution Architect specializing in Azure AI Foundry, Azure OpenAI, AI Agents,
Retrieval-Augmented Generation (RAG), Document Intelligence, Azure AI Search, API Management,
Terraform, AIOps, and enterprise integration. Experienced in architecting scalable, secure,
production-ready AI solutions on Microsoft Azure.
