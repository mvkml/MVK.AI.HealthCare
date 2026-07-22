import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Signup } from './signup';

describe('Signup', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    localStorage.clear();
    await TestBed.configureTestingModule({
      imports: [Signup],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  function create() {
    const fixture = TestBed.createComponent(Signup);
    return fixture.componentInstance;
  }

  it('requires all fields', () => {
    const page = create();
    page.onSubmit();
    expect(page.errorMessage()).toContain('fill in all fields');
    httpMock.expectNone('/api/users/signup');
  });

  it('requires the password and confirmation to match', () => {
    const page = create();
    page.fullName.set('Jane Doe');
    page.email.set('jane@example.com');
    page.password.set('password123');
    page.confirmPassword.set('different123');
    page.onSubmit();
    expect(page.errorMessage()).toContain('do not match');
    httpMock.expectNone('/api/users/signup');
  });

  it('requires at least 8 characters', () => {
    const page = create();
    page.fullName.set('Jane Doe');
    page.email.set('jane@example.com');
    page.password.set('short');
    page.confirmPassword.set('short');
    page.onSubmit();
    expect(page.errorMessage()).toContain('at least 8 characters');
    httpMock.expectNone('/api/users/signup');
  });

  it('sends the persona-mapped RoleId and a placeholder company, then shows the success message', () => {
    const page = create();
    page.fullName.set('Jane Doe');
    page.email.set('jane@example.com');
    page.persona.set('patient');
    page.password.set('password123');
    page.confirmPassword.set('password123');
    page.onSubmit();

    const req = httpMock.expectOne('/api/users/signup');
    expect(req.request.body).toEqual({
      fullName: 'Jane Doe',
      email: 'jane@example.com',
      company: 'N/A',
      password: 'password123',
      roleId: 2
    });
    req.flush({
      isNotValid: false,
      message: '',
      userId: 5,
      fullName: 'Jane Doe',
      email: 'jane@example.com',
      company: 'N/A'
    });

    expect(page.errorMessage()).toBeNull();
    expect(page.successMessage()).toContain('Account created');
  });

  it('surfaces a backend validation error (e.g. duplicate email)', () => {
    const page = create();
    page.fullName.set('Jane Doe');
    page.email.set('jane@example.com');
    page.password.set('password123');
    page.confirmPassword.set('password123');
    page.onSubmit();

    httpMock.expectOne('/api/users/signup').flush(
      { isNotValid: true, message: 'An account with that email already exists.' },
      { status: 400, statusText: 'Bad Request' }
    );

    expect(page.errorMessage()).toContain('already exists');
    expect(page.successMessage()).toBeNull();
  });
});
