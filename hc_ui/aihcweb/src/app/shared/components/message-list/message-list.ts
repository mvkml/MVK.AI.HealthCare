import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MessageItem } from '../message-item/message-item';
import { ChatMessage } from '../../models/chat-message.model';

@Component({
  selector: 'app-message-list',
  imports: [MessageItem],
  templateUrl: './message-list.html',
  styleUrl: './message-list.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MessageList {
  readonly messages = input.required<ChatMessage[]>();
}
