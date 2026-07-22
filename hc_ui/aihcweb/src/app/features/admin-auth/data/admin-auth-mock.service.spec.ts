import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';
import { AdminAuthMockService } from './admin-auth-mock.service';

describe('AdminAuthMockService', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({});
  });

  it('starts logged out', () => {
    const service = TestBed.inject(AdminAuthMockService);
    expect(service.isLoggedIn()).toBe(false);
    expect(service.currentAdmin()).toBeNull();
  });

  it('rejects login before any account has signed up', async () => {
    const service = TestBed.inject(AdminAuthMockService);
    const result = await firstValueFrom(service.login('admin@example.com', 'password123'));
    expect(result.isNotValid).toBe(true);
    expect(service.isLoggedIn()).toBe(false);
  });

  it('signs up then logs in with the same credentials', async () => {
    const service = TestBed.inject(AdminAuthMockService);

    const signupResult = await firstValueFrom(
      service.signUp('Jane Admin', 'admin@example.com', 'password123')
    );
    expect(signupResult.isNotValid).toBe(false);
    expect(service.isLoggedIn()).toBe(false); // signup does not auto-login

    const loginResult = await firstValueFrom(service.login('admin@example.com', 'password123'));
    expect(loginResult.isNotValid).toBe(false);
    expect(service.isLoggedIn()).toBe(true);
    expect(service.currentAdmin()?.fullName).toBe('Jane Admin');
  });

  it('rejects a duplicate signup email', async () => {
    const service = TestBed.inject(AdminAuthMockService);
    await firstValueFrom(service.signUp('Jane Admin', 'admin@example.com', 'password123'));
    const second = await firstValueFrom(
      service.signUp('Other Admin', 'admin@example.com', 'differentPass1')
    );
    expect(second.isNotValid).toBe(true);
    expect(second.message).toContain('already exists');
  });

  it('logout clears the session', async () => {
    const service = TestBed.inject(AdminAuthMockService);
    await firstValueFrom(service.signUp('Jane Admin', 'admin@example.com', 'password123'));
    await firstValueFrom(service.login('admin@example.com', 'password123'));
    expect(service.isLoggedIn()).toBe(true);

    service.logout();
    expect(service.isLoggedIn()).toBe(false);
    expect(service.currentAdmin()).toBeNull();
  });

  it('restores the session from localStorage on a fresh injector', async () => {
    const first = TestBed.inject(AdminAuthMockService);
    await firstValueFrom(first.signUp('Jane Admin', 'admin@example.com', 'password123'));
    await firstValueFrom(first.login('admin@example.com', 'password123'));

    const restored = TestBed.runInInjectionContext(() => new AdminAuthMockService());
    expect(restored.isLoggedIn()).toBe(true);
    expect(restored.currentAdmin()?.email).toBe('admin@example.com');
  });
});
