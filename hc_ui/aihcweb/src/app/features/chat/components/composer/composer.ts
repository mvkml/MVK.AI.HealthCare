import { ChangeDetectionStrategy, Component, ElementRef, output, viewChild } from '@angular/core';

@Component({
  selector: 'app-composer',
  imports: [],
  templateUrl: './composer.html',
  styleUrl: './composer.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Composer {
  readonly send = output<string>();

  private readonly textarea = viewChild.required<ElementRef<HTMLTextAreaElement>>('input');

  onSubmit(event: Event): void {
    event.preventDefault();
    const el = this.textarea().nativeElement;
    const text = el.value.trim();
    if (!text) {
      return;
    }
    this.send.emit(text);
    el.value = '';
    this.autoGrow(el);
  }

  onKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.onSubmit(event);
    }
  }

  onInput(event: Event): void {
    this.autoGrow(event.target as HTMLTextAreaElement);
  }

  private autoGrow(el: HTMLTextAreaElement): void {
    el.style.height = 'auto';
    el.style.height = Math.min(el.scrollHeight, 160) + 'px';
  }
}
