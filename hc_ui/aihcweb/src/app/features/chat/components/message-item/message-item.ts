import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { ChatMessage } from '../../models/chat-message.model';

@Component({
  selector: 'app-message-item',
  imports: [],
  templateUrl: './message-item.html',
  styleUrl: './message-item.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class.msg-user]': "message().role === 'user'",
    '[class.msg-assistant]': "message().role === 'assistant'"
  }
})
export class MessageItem {
  readonly message = input.required<ChatMessage>();
}
