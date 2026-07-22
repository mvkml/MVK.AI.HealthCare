import { Injectable, computed, signal } from '@angular/core';
import { Observable, of } from 'rxjs';
import { AdminAuthResult, AdminUser } from '../models/admin-auth.model';

// Mock only — no Admin role exists in HC.AI.Identity.Api yet (Roles table is just
// 1 Doctor / 2 Patient), and no admin-management endpoints exist to wire this to.
// See BACKLOG PB031 / TASK015. Same shape the original AuthMockService had before the real
// Identity API existed: an in-memory/localStorage-backed account list, not a live backend.

const ACCOUNTS_KEY = 'hc_admin_accounts';
const SESSION_KEY = 'hc_admin_auth_session';

interface StoredAdminAccount extends AdminUser {
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AdminAuthMockService {
  readonly currentAdmin = signal<AdminUser | null>(this.loadSession());
  readonly isLoggedIn = computed(() => this.currentAdmin() !== null);

  login(email: string, password: string): Observable<AdminAuthResult> {
    const normalizedEmail = email.trim().toLowerCase();
    const account = this.loadAccounts().find(
      (a) => a.email === normalizedEmail && a.password === password
    );

    if (!account) {
      return of({ isNotValid: true, message: 'Invalid email or password.' });
    }

    const user: AdminUser = { id: account.id, fullName: account.fullName, email: account.email };
    this.currentAdmin.set(user);
    localStorage.setItem(SESSION_KEY, JSON.stringify(user));
    return of({ isNotValid: false, message: '', user });
  }

  signUp(fullName: string, email: string, password: string): Observable<AdminAuthResult> {
    const normalizedEmail = email.trim().toLowerCase();
    const accounts = this.loadAccounts();

    if (accounts.some((a) => a.email === normalizedEmail)) {
      return of({ isNotValid: true, message: 'An admin account with that email already exists.' });
    }

    const account: StoredAdminAccount = {
      id: crypto.randomUUID(),
      fullName,
      email: normalizedEmail,
      password
    };
    accounts.push(account);
    localStorage.setItem(ACCOUNTS_KEY, JSON.stringify(accounts));

    return of({ isNotValid: false, message: 'Admin account created — you can now sign in.' });
  }

  logout(): void {
    this.currentAdmin.set(null);
    localStorage.removeItem(SESSION_KEY);
  }

  private loadAccounts(): StoredAdminAccount[] {
    const raw = localStorage.getItem(ACCOUNTS_KEY);
    if (!raw) return [];
    try {
      return JSON.parse(raw) as StoredAdminAccount[];
    } catch {
      return [];
    }
  }

  private loadSession(): AdminUser | null {
    const raw = localStorage.getItem(SESSION_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as AdminUser;
    } catch {
      return null;
    }
  }
}
