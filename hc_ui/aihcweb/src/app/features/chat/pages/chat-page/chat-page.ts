import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { ChatRail } from '../../components/chat-rail/chat-rail';
import { MessageList } from '../../components/message-list/message-list';
import { Composer } from '../../components/composer/composer';
import { ChatMessage } from '../../models/chat-message.model';
import { MOCK_CHAT_HISTORY } from '../../data/chat-mock-data';

@Component({
  selector: 'app-chat-page',
  imports: [ChatRail, MessageList, Composer],
  templateUrl: './chat-page.html',
  styleUrl: './chat-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ChatPage {
  readonly messages = signal<ChatMessage[]>(MOCK_CHAT_HISTORY);

  onSend(text: string): void {
    const time = this.formatTime(new Date());

    this.messages.update((current) => [
      ...current,
      { id: crypto.randomUUID(), role: 'user', time, text }
    ]);

    // Mock reply only — no HC.AI.MAPI call yet, pending the US007 endpoint contract.
    this.messages.update((current) => [
      ...current,
      {
        id: crypto.randomUUID(),
        role: 'assistant',
        time,
        text: '(Mock data — no backend wired yet. This is where the US007 Chat REST API response renders.)'
      }
    ]);
  }

  private formatTime(date: Date): string {
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  }
}
