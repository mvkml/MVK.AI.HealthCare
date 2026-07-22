import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

// Mirrors HC.AI.MAPI.Models.Prompt.PromptRequest / PromptResponse (Module 1, US007).
// See hc_agile/worklogs/dev_semantic_kernel/20260718_180408_module1_doctor_prompt_locked.md
//
// `persona` added per PB034 — DoctorPromptMapper (C#) now reads this to pick the executor
// model (APIConstants.DoctorExecutorPersonaName vs PatientExecutorPersonaName). Sent explicitly
// rather than left to the server default, since this endpoint may eventually be shared beyond
// just Doctor requests.
export interface PromptRequest {
  message: string;
  persona: string;
  maxTokens: number;
  temperature: number;
  topP: number;
  topK: number;
  frequencyPenalty: number;
  presencePenalty: number;
  stopSequences: string[];
  stream: boolean;
  seed: number | null;
}

export interface PromptResponse {
  content: string;
  finishReason: string;
  promptTokens: number;
  completionTokens: number;
  totalTokens: number;
  modelUsed: string;
  latencyMs: number;
  isSuccess: boolean;
  errorMessage: string;
}

const DEFAULT_GENERATION_CONTROLS: Omit<PromptRequest, 'message'> = {
  persona: 'Doctor',
  maxTokens: 200,
  temperature: 0.3,
  topP: 0.9,
  topK: 40,
  frequencyPenalty: 0,
  presencePenalty: 0,
  stopSequences: [],
  stream: false,
  seed: null
};

@Injectable({ providedIn: 'root' })
export class DoctorChatService {
  private readonly http = inject(HttpClient);

  providePrompt(message: string): Observable<PromptResponse> {
    const request: PromptRequest = { message, ...DEFAULT_GENERATION_CONTROLS };
    return this.http.post<PromptResponse>('/api/Doctor/provide-prompt', request);
  }
}
