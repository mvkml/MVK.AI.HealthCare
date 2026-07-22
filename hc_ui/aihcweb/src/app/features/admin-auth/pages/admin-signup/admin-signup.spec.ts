import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AdminSignup } from './admin-signup';

describe('AdminSignup', () => {
  beforeEach(async () => {
    localStorage.clear();
    await TestBed.configureTestingModule({
      imports: [AdminSignup],
      providers: [provideRouter([])]
    }).compileComponents();
  });

  function create() {
    const fixture = TestBed.createComponent(AdminSignup);
    return fixture.componentInstance;
  }

  it('requires all fields', () => {
    const page = create();
    page.onSubmit();
    expect(page.errorMessage()).toContain('fill in all fields');
  });

  it('requires the password and confirmation to match', () => {
    const page = create();
    page.fullName.set('Jane Admin');
    page.email.set('admin@example.com');
    page.password.set('password123');
    page.confirmPassword.set('different123');
    page.onSubmit();
    expect(page.errorMessage()).toContain('do not match');
  });

  it('requires at least 8 characters', () => {
    const page = create();
    page.fullName.set('Jane Admin');
    page.email.set('admin@example.com');
    page.password.set('short');
    page.confirmPassword.set('short');
    page.onSubmit();
    expect(page.errorMessage()).toContain('at least 8 characters');
  });

  it('creates the account and shows the success message', () => {
    const page = create();
    page.fullName.set('Jane Admin');
    page.email.set('admin@example.com');
    page.password.set('password123');
    page.confirmPassword.set('password123');
    page.onSubmit();

    expect(page.errorMessage()).toBeNull();
    expect(page.successMessage()).toContain('Admin account created');
  });

  it('surfaces a duplicate-email error', () => {
    const page = create();
    page.fullName.set('Jane Admin');
    page.email.set('admin@example.com');
    page.password.set('password123');
    page.confirmPassword.set('password123');
    page.onSubmit();
    expect(page.successMessage()).not.toBeNull();

    page.successMessage.set(null);
    page.onSubmit();

    expect(page.errorMessage()).toContain('already exists');
    expect(page.successMessage()).toBeNull();
  });
});
