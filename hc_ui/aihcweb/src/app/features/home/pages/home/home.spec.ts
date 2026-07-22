import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { Home } from './home';
import { AuthService } from '../../../auth/data/auth.service';

describe('Home', () => {
  beforeEach(async () => {
    localStorage.clear();
    await TestBed.configureTestingModule({
      imports: [Home],
      providers: [
        provideRouter([{ path: 'login', component: Home }]),
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();
  });

  it('shows the Doctor Chat action card for a doctor', () => {
    const auth = TestBed.inject(AuthService);
    auth.currentUser.set({ id: '1', fullName: 'Dr. Rosalind Okafor', email: 'doctor@example.com', persona: 'doctor' });

    const fixture = TestBed.createComponent(Home);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;

    expect(compiled.textContent).toContain('Dr. Rosalind Okafor');
    expect(compiled.textContent).toContain('Doctor Chat');
    expect(compiled.querySelector('a[href="/chat"]')).toBeTruthy();
  });

  it('shows the My Health Assistant action card for a patient, linking to /patient-chat', () => {
    const auth = TestBed.inject(AuthService);
    auth.currentUser.set({ id: '2', fullName: 'Marcus Webb', email: 'patient@example.com', persona: 'patient' });

    const fixture = TestBed.createComponent(Home);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;

    expect(compiled.textContent).toContain('My Health Assistant');
    expect(compiled.querySelector('a[href="/patient-chat"]')).toBeTruthy();
    expect(compiled.querySelector('a[href="/chat"]')).toBeFalsy();
  });

  it('logs out and navigates to /login', () => {
    const auth = TestBed.inject(AuthService);
    auth.currentUser.set({ id: '1', fullName: 'Dr. Rosalind Okafor', email: 'doctor@example.com', persona: 'doctor' });

    const fixture = TestBed.createComponent(Home);
    fixture.detectChanges();
    fixture.componentInstance.logout();

    expect(auth.isLoggedIn()).toBe(false);
  });
});
