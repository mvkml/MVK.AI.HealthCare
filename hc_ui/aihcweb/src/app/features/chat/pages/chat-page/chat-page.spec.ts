import { TestBed } from '@angular/core/testing';
import { ChatPage } from './chat-page';
import { MOCK_CHAT_HISTORY } from '../../data/chat-mock-data';

describe('ChatPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChatPage]
    }).compileComponents();
  });

  it('starts with the mock chat history', () => {
    const fixture = TestBed.createComponent(ChatPage);
    const page = fixture.componentInstance;
    expect(page.messages()).toEqual(MOCK_CHAT_HISTORY);
  });

  it('renders the mock messages in the DOM', () => {
    const fixture = TestBed.createComponent(ChatPage);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelectorAll('app-message-item').length).toBe(MOCK_CHAT_HISTORY.length);
  });

  it('appends a user message and a mock assistant reply on send', () => {
    const fixture = TestBed.createComponent(ChatPage);
    const page = fixture.componentInstance;
    const startCount = page.messages().length;

    page.onSend('How many patients today?');

    const messages = page.messages();
    expect(messages.length).toBe(startCount + 2);
    expect(messages.at(-2)).toMatchObject({ role: 'user', text: 'How many patients today?' });
    expect(messages.at(-1)).toMatchObject({ role: 'assistant' });
  });
});
