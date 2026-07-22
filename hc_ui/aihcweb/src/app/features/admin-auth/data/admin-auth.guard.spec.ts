import { TestBed } from '@angular/core/testing';
import { UrlTree, provideRouter } from '@angular/router';
import { adminAuthGuard } from './admin-auth.guard';
import { AdminAuthMockService } from './admin-auth-mock.service';

describe('adminAuthGuard', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideRouter([])]
    });
  });

  it('allows navigation when an admin is logged in', () => {
    const auth = TestBed.inject(AdminAuthMockService);
    auth.currentAdmin.set({ id: '1', fullName: 'Jane Admin', email: 'admin@example.com' });

    const result = TestBed.runInInjectionContext(() => adminAuthGuard({} as never, {} as never));
    expect(result).toBe(true);
  });

  it('redirects to /admin/login when logged out', () => {
    const result = TestBed.runInInjectionContext(() => adminAuthGuard({} as never, {} as never));
    expect(result instanceof UrlTree).toBe(true);
    expect((result as UrlTree).toString()).toBe('/admin/login');
  });
});
