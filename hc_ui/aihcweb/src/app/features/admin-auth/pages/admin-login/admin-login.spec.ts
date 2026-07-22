import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AdminLogin } from './admin-login';
import { AdminAuthMockService } from '../../data/admin-auth-mock.service';

describe('AdminLogin', () => {
  beforeEach(async () => {
    localStorage.clear();
    await TestBed.configureTestingModule({
      imports: [AdminLogin],
      providers: [provideRouter([])]
    }).compileComponents();
  });

  it('shows a validation error for an empty form', () => {
    const fixture = TestBed.createComponent(AdminLogin);
    const page = fixture.componentInstance;

    page.onSubmit();

    expect(page.errorMessage()).toContain('enter your email and password');
    expect(page.isLoading()).toBe(false);
  });

  it('logs in successfully and navigates to /admin/home', async () => {
    const auth = TestBed.inject(AdminAuthMockService);
    await firstValueFrom(auth.signUp('Jane Admin', 'admin@example.com', 'password123'));

    const fixture = TestBed.createComponent(AdminLogin);
    const page = fixture.componentInstance;
    const router = TestBed.inject(Router);
    const navigateSpy = vi.spyOn(router, 'navigate');

    page.email.set('admin@example.com');
    page.password.set('password123');
    page.onSubmit();

    expect(page.isLoading()).toBe(false);
    expect(page.errorMessage()).toBeNull();
    expect(navigateSpy).toHaveBeenCalledWith(['/admin/home']);
  });

  it('shows an error for invalid credentials without navigating', () => {
    const fixture = TestBed.createComponent(AdminLogin);
    const page = fixture.componentInstance;
    const router = TestBed.inject(Router);
    const navigateSpy = vi.spyOn(router, 'navigate');

    page.email.set('admin@example.com');
    page.password.set('wrong');
    page.onSubmit();

    expect(page.errorMessage()).toBe('Invalid email or password.');
    expect(navigateSpy).not.toHaveBeenCalled();
  });
});
