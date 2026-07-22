import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AdminHome } from './admin-home';
import { AdminAuthMockService } from '../../data/admin-auth-mock.service';

describe('AdminHome', () => {
  beforeEach(async () => {
    localStorage.clear();
    await TestBed.configureTestingModule({
      imports: [AdminHome],
      providers: [provideRouter([{ path: 'admin/login', component: AdminHome }])]
    }).compileComponents();
  });

  it('shows the logged-in admin name', () => {
    const auth = TestBed.inject(AdminAuthMockService);
    auth.currentAdmin.set({ id: '1', fullName: 'Jane Admin', email: 'admin@example.com' });

    const fixture = TestBed.createComponent(AdminHome);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;

    expect(compiled.textContent).toContain('Jane Admin');
  });

  it('logs out and clears the session', () => {
    const auth = TestBed.inject(AdminAuthMockService);
    auth.currentAdmin.set({ id: '1', fullName: 'Jane Admin', email: 'admin@example.com' });

    const fixture = TestBed.createComponent(AdminHome);
    fixture.detectChanges();
    fixture.componentInstance.logout();

    expect(auth.isLoggedIn()).toBe(false);
  });
});
