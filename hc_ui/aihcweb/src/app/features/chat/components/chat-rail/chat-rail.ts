import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MOCK_HISTORY_ITEMS, MOCK_PERSONA } from '../../data/chat-mock-data';

@Component({
  selector: 'app-chat-rail',
  imports: [],
  templateUrl: './chat-rail.html',
  styleUrl: './chat-rail.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ChatRail {
  readonly historyItems = input(MOCK_HISTORY_ITEMS);
  readonly persona = input(MOCK_PERSONA);

  get historyGroups(): string[] {
    return [...new Set(this.historyItems().map((item) => item.group))];
  }

  itemsFor(group: string) {
    return this.historyItems().filter((item) => item.group === group);
  }
}
