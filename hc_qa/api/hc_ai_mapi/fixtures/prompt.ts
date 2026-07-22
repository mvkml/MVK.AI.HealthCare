export interface PromptRequestBody {
  message: string;
  persona?: string;
  maxTokens?: number;
  temperature?: number;
  topP?: number;
  topK?: number;
}

/** Builds a `POST /api/Doctor/provide-prompt` body. Persona omitted by default, matching the "no persona sent" case. */
export function buildPromptRequest(overrides: Partial<PromptRequestBody> = {}): PromptRequestBody {
  return {
    message: 'hi',
    ...overrides,
  };
}
