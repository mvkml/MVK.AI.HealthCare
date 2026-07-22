import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { ChatRail } from '../../../../shared/components/chat-rail/chat-rail';
import { MessageList } from '../../../../shared/components/message-list/message-list';
import { Composer } from '../../../../shared/components/composer/composer';
import { ChatMessage } from '../../../../shared/models/chat-message.model';
import {
  MOCK_PATIENT_CHAT_HISTORY,
  MOCK_PATIENT_HISTORY_ITEMS,
  MOCK_PATIENT_PERSONA
} from '../../data/patient-chat-mock-data';
import { PatientChatService } from '../../data/patient-chat.service';
import { AuthService } from '../../../auth/data/auth.service';

@Component({
  selector: 'app-patient-chat-page',
  imports: [ChatRail, MessageList, Composer],
  templateUrl: './patient-chat-page.html',
  styleUrl: './patient-chat-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PatientChatPage {
  private readonly patientChat = inject(PatientChatService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly messages = signal<ChatMessage[]>(MOCK_PATIENT_CHAT_HISTORY);
  readonly historyItems = MOCK_PATIENT_HISTORY_ITEMS;
  readonly isSending = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly railPersona = computed(() => {
    const user = this.auth.currentUser();
    if (!user) {
      return MOCK_PATIENT_PERSONA;
    }
    return { name: user.fullName, role: 'Patient' };
  });

  onLogout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  onSend(text: string): void {
    const time = this.formatTime(new Date());

    this.messages.update((current) => [
      ...current,
      { id: crypto.randomUUID(), role: 'user', time, text }
    ]);

    this.isSending.set(true);
    this.errorMessage.set(null);

    this.patientChat.providePrompt(text).subscribe({
      next: (response) => {
        this.isSending.set(false);
        const replyTime = this.formatTime(new Date());
        if (!response.isSuccess) {
          this.errorMessage.set(response.errorMessage || 'The assistant could not answer that.');
          return;
        }
        this.messages.update((current) => [
          ...current,
          {
            id: crypto.randomUUID(),
            role: 'assistant',
            time: replyTime,
            text: response.content,
            sourceTag: `${response.modelUsed} · ${response.latencyMs} ms`
          }
        ]);
      },
      error: (err: HttpErrorResponse) => {
        this.isSending.set(false);
        if (err.status === 0) {
          this.errorMessage.set(
            'Could not reach the Patient chat API. Is HC.AI.MAPI running on http://localhost:5150?'
          );
        } else if (Array.isArray(err.error)) {
          // 400 validation failure — HC.AI.MAPI returns a plain string array, not a PromptResponse.
          this.errorMessage.set(err.error.join(' '));
        } else {
          this.errorMessage.set(`Patient chat API error (HTTP ${err.status}).`);
        }
      }
    });
  }

  private formatTime(date: Date): string {
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  }
}
