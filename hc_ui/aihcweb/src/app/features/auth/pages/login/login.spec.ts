import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Login } from './login';

describe('Login', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    localStorage.clear();
    await TestBed.configureTestingModule({
      imports: [Login],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('shows a validation error and does not call the auth API for an empty form', () => {
    const fixture = TestBed.createComponent(Login);
    const page = fixture.componentInstance;

    page.onSubmit();

    expect(page.errorMessage()).toContain('enter your email and password');
    expect(page.isLoading()).toBe(false);
    httpMock.expectNone('/api/users/login');
  });

  it('logs in successfully and navigates to /home', () => {
    const fixture = TestBed.createComponent(Login);
    const page = fixture.componentInstance;
    const router = TestBed.inject(Router);
    const navigateSpy = vi.spyOn(router, 'navigate');

    page.email.set('doctor@example.com');
    page.password.set('password123');
    page.onSubmit();

    expect(page.isLoading()).toBe(true);

    httpMock.expectOne('/api/users/login').flush({
      isNotValid: false,
      message: '',
      userId: 1,
      fullName: 'Dr. Test',
      email: 'doctor@example.com',
      company: 'N/A',
      roleId: 1,
      token: 'fake.jwt.token'
    });

    expect(page.isLoading()).toBe(false);
    expect(page.errorMessage()).toBeNull();
    expect(navigateSpy).toHaveBeenCalledWith(['/home']);
  });

  it('shows the backend error for invalid credentials without navigating', () => {
    const fixture = TestBed.createComponent(Login);
    const page = fixture.componentInstance;
    const router = TestBed.inject(Router);
    const navigateSpy = vi.spyOn(router, 'navigate');

    page.email.set('doctor@example.com');
    page.password.set('wrong');
    page.onSubmit();

    httpMock.expectOne('/api/users/login').flush(
      { isNotValid: true, message: 'Invalid email or password.' },
      { status: 401, statusText: 'Unauthorized' }
    );

    expect(page.errorMessage()).toBe('Invalid email or password.');
    expect(navigateSpy).not.toHaveBeenCalled();
  });
});
