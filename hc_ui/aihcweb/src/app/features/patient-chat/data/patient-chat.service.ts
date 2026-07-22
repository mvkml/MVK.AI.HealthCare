import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

// Mirrors HC.AI.MAPI.Models.Prompt.PromptRequest / PromptResponse, same shape as
// DoctorChatService (features/chat/data/doctor-chat.service.ts) — Patient hits its own
// controller/service/mapper chain server-side (PatientController -> PatientService ->
// PatientPromptMapper), which hardcodes persona/model selection itself rather than trusting
// a client-supplied value (see PatientPromptMapper's doc-comment / US021).
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
  persona: 'Patient',
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
export class PatientChatService {
  private readonly http = inject(HttpClient);

  providePrompt(message: string): Observable<PromptResponse> {
    const request: PromptRequest = { message, ...DEFAULT_GENERATION_CONTROLS };
    return this.http.post<PromptResponse>('/api/Patient/provide-prompt', request);
  }
}
