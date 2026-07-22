import { TestBed } from '@angular/core/testing';
import { ChatRail } from './chat-rail';

describe('ChatRail', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChatRail]
    }).compileComponents();
  });

  function create() {
    const fixture = TestBed.createComponent(ChatRail);
    fixture.componentRef.setInput('historyItems', [{ label: 'Today', group: 'Today', active: true }]);
    fixture.componentRef.setInput('persona', { name: 'Test User', role: 'Doctor' });
    fixture.detectChanges();
    return fixture;
  }

  it('emits logout when the Log out button is clicked', () => {
    const fixture = create();
    let emitted = false;
    fixture.componentInstance.logout.subscribe(() => (emitted = true));

    const button = Array.from(fixture.nativeElement.querySelectorAll('button')).find(
      (b) => (b as HTMLButtonElement).textContent?.trim() === 'Log out'
    ) as HTMLButtonElement;
    button.click();

    expect(emitted).toBe(true);
  });
});
