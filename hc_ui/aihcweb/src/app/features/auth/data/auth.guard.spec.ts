import { TestBed } from '@angular/core/testing';
import { UrlTree, provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { authGuard } from './auth.guard';
import { AuthService } from './auth.service';

describe('authGuard', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()]
    });
  });

  it('allows navigation when logged in', () => {
    const auth = TestBed.inject(AuthService);
    auth.currentUser.set({ id: '1', fullName: 'Dr. Test', email: 'doctor@example.com', persona: 'doctor' });

    const result = TestBed.runInInjectionContext(() => authGuard({} as never, {} as never));
    expect(result).toBe(true);
  });

  it('redirects to /login when logged out', () => {
    const result = TestBed.runInInjectionContext(() => authGuard({} as never, {} as never));
    expect(result instanceof UrlTree).toBe(true);
    expect((result as UrlTree).toString()).toBe('/login');
  });
});
