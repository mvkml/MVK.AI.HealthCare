import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('starts logged out', () => {
    expect(service.isLoggedIn()).toBe(false);
    expect(service.currentUser()).toBeNull();
  });

  it('logs in successfully and maps RoleId 1 to the doctor persona', () => {
    let result: { isNotValid: boolean } | undefined;
    service.login('doctor@example.com', 'password123').subscribe((r) => (result = r));

    const req = httpMock.expectOne('/api/users/login');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'doctor@example.com', password: 'password123' });
    req.flush({
      isNotValid: false,
      message: '',
      userId: 1,
      fullName: 'Dr. Test',
      email: 'doctor@example.com',
      company: 'N/A',
      roleId: 1,
      token: 'fake.jwt.token'
    });

    expect(result?.isNotValid).toBe(false);
    expect(service.isLoggedIn()).toBe(true);
    expect(service.currentUser()).toEqual({
      id: '1',
      fullName: 'Dr. Test',
      email: 'doctor@example.com',
      persona: 'doctor'
    });
  });

  it('maps RoleId 2 to the patient persona', () => {
    service.login('patient@example.com', 'password123').subscribe();
    const req = httpMock.expectOne('/api/users/login');
    req.flush({
      isNotValid: false,
      message: '',
      userId: 2,
      fullName: 'Pat Test',
      email: 'patient@example.com',
      company: 'N/A',
      roleId: 2,
      token: 'fake.jwt.token'
    });

    expect(service.currentUser()?.persona).toBe('patient');
  });

  it('surfaces a 401 invalid-credentials response without logging in', () => {
    let result: { isNotValid: boolean; message: string } | undefined;
    service.login('doctor@example.com', 'wrong').subscribe((r) => (result = r));

    const req = httpMock.expectOne('/api/users/login');
    req.flush(
      { isNotValid: true, message: 'Invalid email or password.' },
      { status: 401, statusText: 'Unauthorized' }
    );

    expect(result?.isNotValid).toBe(true);
    expect(result?.message).toBe('Invalid email or password.');
    expect(service.isLoggedIn()).toBe(false);
  });

  it('reports a network failure distinctly from a validation failure', () => {
    let result: { isNotValid: boolean; message: string } | undefined;
    service.login('doctor@example.com', 'password123').subscribe((r) => (result = r));

    const req = httpMock.expectOne('/api/users/login');
    req.error(new ProgressEvent('network error'));

    expect(result?.isNotValid).toBe(true);
    expect(result?.message).toContain('Could not reach the Identity API');
  });

  it('signs up with the persona mapped to RoleId and a placeholder company', () => {
    let result: { isNotValid: boolean; message: string } | undefined;
    service.signUp('New Patient', 'new.patient@example.com', 'password123', 'patient').subscribe((r) => (result = r));

    const req = httpMock.expectOne('/api/users/signup');
    expect(req.request.body).toEqual({
      fullName: 'New Patient',
      email: 'new.patient@example.com',
      company: 'N/A',
      password: 'password123',
      roleId: 2
    });
    req.flush({
      isNotValid: false,
      message: '',
      userId: 3,
      fullName: 'New Patient',
      email: 'new.patient@example.com',
      company: 'N/A'
    });

    expect(result?.isNotValid).toBe(false);
    expect(result?.message).toContain('Account created');
    // Sign-up does not auto-login.
    expect(service.isLoggedIn()).toBe(false);
  });

  it('surfaces a signup validation error (e.g. duplicate email)', () => {
    let result: { isNotValid: boolean; message: string } | undefined;
    service.signUp('New Patient', 'existing@example.com', 'password123', 'patient').subscribe((r) => (result = r));

    const req = httpMock.expectOne('/api/users/signup');
    req.flush(
      { isNotValid: true, message: 'An account with that email already exists.' },
      { status: 400, statusText: 'Bad Request' }
    );

    expect(result?.isNotValid).toBe(true);
    expect(result?.message).toContain('already exists');
  });

  it('logs out and clears the session', () => {
    service.login('doctor@example.com', 'password123').subscribe();
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
    expect(service.isLoggedIn()).toBe(true);

    service.logout();
    expect(service.isLoggedIn()).toBe(false);
    expect(service.currentUser()).toBeNull();
  });

  it('restores the session from localStorage on construction', async () => {
    service.login('doctor@example.com', 'password123').subscribe();
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

    // Simulate a page reload: a fresh injector, same localStorage.
    const freshService = TestBed.runInInjectionContext(() => new AuthService());
    expect(freshService.currentUser()?.email).toBe('doctor@example.com');
  });
});
