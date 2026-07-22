import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { PatientChatPage } from './patient-chat-page';
import { MOCK_PATIENT_CHAT_HISTORY } from '../../data/patient-chat-mock-data';
import { PromptResponse } from '../../data/patient-chat.service';
import { AuthService } from '../../../auth/data/auth.service';

describe('PatientChatPage', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    localStorage.clear();
    await TestBed.configureTestingModule({
      imports: [PatientChatPage],
      providers: [
        provideRouter([{ path: 'login', component: PatientChatPage }]),
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('starts with the mock patient chat history', () => {
    const fixture = TestBed.createComponent(PatientChatPage);
    const page = fixture.componentInstance;
    expect(page.messages()).toEqual(MOCK_PATIENT_CHAT_HISTORY);
  });

  it('reflects the real logged-in patient in the rail persona', () => {
    const auth = TestBed.inject(AuthService);
    auth.currentUser.set({ id: '2', fullName: 'Marcus Webb', email: 'patient@example.com', persona: 'patient' });

    const fixture = TestBed.createComponent(PatientChatPage);
    const page = fixture.componentInstance;

    expect(page.railPersona()).toEqual({ name: 'Marcus Webb', role: 'Patient' });
  });

  it('sends the message to the Patient chat API and appends the real reply', () => {
    const fixture = TestBed.createComponent(PatientChatPage);
    const page = fixture.componentInstance;
    const startCount = page.messages().length;

    page.onSend('When is my next appointment?');

    expect(page.messages().length).toBe(startCount + 1);
    expect(page.messages().at(-1)).toMatchObject({ role: 'user', text: 'When is my next appointment?' });
    expect(page.isSending()).toBe(true);

    const req = httpMock.expectOne('/api/Patient/provide-prompt');
    expect(req.request.method).toBe('POST');
    expect(req.request.body.message).toBe('When is my next appointment?');
    expect(req.request.body.persona).toBe('Patient');

    const response: PromptResponse = {
      content: 'Your next appointment is Jul 22, 2026 with Dr. Rosalind Okafor.',
      finishReason: '',
      promptTokens: 0,
      completionTokens: 0,
      totalTokens: 0,
      modelUsed: 'hc-patient-executor:1.1',
      latencyMs: 400,
      isSuccess: true,
      errorMessage: ''
    };
    req.flush(response);

    expect(page.isSending()).toBe(false);
    expect(page.messages().length).toBe(startCount + 2);
    expect(page.messages().at(-1)).toMatchObject({
      role: 'assistant',
      text: 'Your next appointment is Jul 22, 2026 with Dr. Rosalind Okafor.'
    });
  });

  it('shows an error and does not append a reply when the API call fails', () => {
    const fixture = TestBed.createComponent(PatientChatPage);
    const page = fixture.componentInstance;
    const startCount = page.messages().length;

    page.onSend('hi');

    const req = httpMock.expectOne('/api/Patient/provide-prompt');
    req.error(new ProgressEvent('network error'));

    expect(page.isSending()).toBe(false);
    expect(page.errorMessage()).toContain('Could not reach');
    expect(page.messages().length).toBe(startCount + 1);
  });

  it('surfaces HC.AI.MAPI validation errors (400, plain string array body)', () => {
    const fixture = TestBed.createComponent(PatientChatPage);
    const page = fixture.componentInstance;

    page.onSend('hi');

    const req = httpMock.expectOne('/api/Patient/provide-prompt');
    req.flush(['Prompt text (Message) is required.'], { status: 400, statusText: 'Bad Request' });

    expect(page.errorMessage()).toBe('Prompt text (Message) is required.');
  });

  it('logs out on demand', () => {
    const auth = TestBed.inject(AuthService);
    auth.currentUser.set({ id: '2', fullName: 'Marcus Webb', email: 'patient@example.com', persona: 'patient' });

    const fixture = TestBed.createComponent(PatientChatPage);
    fixture.componentInstance.onLogout();

    expect(auth.isLoggedIn()).toBe(false);
  });
});
