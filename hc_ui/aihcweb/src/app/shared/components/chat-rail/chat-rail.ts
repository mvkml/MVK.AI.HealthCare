import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

export interface ChatHistoryItem {
  label: string;
  group: string;
  active: boolean;
}

export interface ChatRailPersona {
  name: string;
  role: string;
}

@Component({
  selector: 'app-chat-rail',
  imports: [],
  templateUrl: './chat-rail.html',
  styleUrl: './chat-rail.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ChatRail {
  readonly historyItems = input.required<ChatHistoryItem[]>();
  readonly persona = input.required<ChatRailPersona>();
  readonly logout = output<void>();

  get historyGroups(): string[] {
    return [...new Set(this.historyItems().map((item) => item.group))];
  }

  itemsFor(group: string) {
    return this.historyItems().filter((item) => item.group === group);
  }
}
