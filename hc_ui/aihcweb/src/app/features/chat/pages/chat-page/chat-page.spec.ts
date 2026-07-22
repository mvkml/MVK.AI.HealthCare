import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { ChatPage } from './chat-page';
import { MOCK_CHAT_HISTORY } from '../../data/chat-mock-data';
import { PromptResponse } from '../../data/doctor-chat.service';
import { AuthService } from '../../../auth/data/auth.service';

describe('ChatPage', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    localStorage.clear();
    await TestBed.configureTestingModule({
      imports: [ChatPage],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([{ path: 'login', component: ChatPage }])
      ]
    }).compileComponents();
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
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

  it('sends the message to the Doctor chat API and appends the real reply', () => {
    const fixture = TestBed.createComponent(ChatPage);
    const page = fixture.componentInstance;
    const startCount = page.messages().length;

    page.onSend('How many patients today?');

    expect(page.messages().length).toBe(startCount + 1);
    expect(page.messages().at(-1)).toMatchObject({ role: 'user', text: 'How many patients today?' });
    expect(page.isSending()).toBe(true);

    const req = httpMock.expectOne('/api/Doctor/provide-prompt');
    expect(req.request.method).toBe('POST');
    expect(req.request.body.message).toBe('How many patients today?');

    const response: PromptResponse = {
      content: '6 patients were seen recently.',
      finishReason: '',
      promptTokens: 0,
      completionTokens: 0,
      totalTokens: 0,
      modelUsed: 'qwen2.5:7b',
      latencyMs: 500,
      isSuccess: true,
      errorMessage: ''
    };
    req.flush(response);

    expect(page.isSending()).toBe(false);
    expect(page.messages().length).toBe(startCount + 2);
    expect(page.messages().at(-1)).toMatchObject({
      role: 'assistant',
      text: '6 patients were seen recently.'
    });
  });

  it('shows an error and does not append a reply when the API call fails', () => {
    const fixture = TestBed.createComponent(ChatPage);
    const page = fixture.componentInstance;
    const startCount = page.messages().length;

    page.onSend('hi');

    const req = httpMock.expectOne('/api/Doctor/provide-prompt');
    req.error(new ProgressEvent('network error'));

    expect(page.isSending()).toBe(false);
    expect(page.errorMessage()).toContain('Could not reach');
    expect(page.messages().length).toBe(startCount + 1);
  });

  it('surfaces HC.AI.MAPI validation errors (400, plain string array body)', () => {
    const fixture = TestBed.createComponent(ChatPage);
    const page = fixture.componentInstance;

    page.onSend('hi');

    const req = httpMock.expectOne('/api/Doctor/provide-prompt');
    req.flush(['Prompt text (Message) is required.'], { status: 400, statusText: 'Bad Request' });

    expect(page.errorMessage()).toBe('Prompt text (Message) is required.');
  });

  it('reflects the real logged-in user in the rail persona, and logs out on demand', () => {
    const auth = TestBed.inject(AuthService);
    auth.currentUser.set({ id: '1', fullName: 'Dr. Rosalind Okafor', email: 'doctor@example.com', persona: 'doctor' });

    const fixture = TestBed.createComponent(ChatPage);
    const page = fixture.componentInstance;

    expect(page.railPersona()).toEqual({ name: 'Dr. Rosalind Okafor', role: 'Doctor' });

    page.onLogout();
    expect(auth.isLoggedIn()).toBe(false);
  });
});
