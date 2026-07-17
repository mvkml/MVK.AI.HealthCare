import { TestBed } from '@angular/core/testing';
import { Composer } from './composer';

describe('Composer', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Composer]
    }).compileComponents();
  });

  it('emits the trimmed textarea value on submit', () => {
    const fixture = TestBed.createComponent(Composer);
    fixture.detectChanges();
    const emitted: string[] = [];
    fixture.componentInstance.send.subscribe((text) => emitted.push(text));

    const textarea = fixture.nativeElement.querySelector('textarea') as HTMLTextAreaElement;
    textarea.value = '  how many patients today?  ';
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));

    expect(emitted).toEqual(['how many patients today?']);
    expect(textarea.value).toBe('');
  });

  it('does not emit for an empty or whitespace-only message', () => {
    const fixture = TestBed.createComponent(Composer);
    fixture.detectChanges();
    const emitted: string[] = [];
    fixture.componentInstance.send.subscribe((text) => emitted.push(text));

    const textarea = fixture.nativeElement.querySelector('textarea') as HTMLTextAreaElement;
    textarea.value = '   ';
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));

    expect(emitted).toEqual([]);
  });
});
