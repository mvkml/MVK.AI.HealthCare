import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, catchError, map, of } from 'rxjs';
import { AuthResult, AuthUser, Persona, RoleOption } from '../models/auth.model';

// Real backend — HC.AI.Identity.Api (hc_ai_in/mapi/HC.AI.Identity.Api), proxied via
// proxy.conf.json (/api/users -> http://localhost:5008). See TASK013 / PB025.
//
// Two things intentionally NOT solved here, flagged rather than silently decided:
// - Token storage: the API returns the JWT in the response body (not a Set-Cookie), so
//   localStorage is the only option that works without a backend change. httpOnly-cookie
//   storage was flagged in TASK013 as needing Architect/PO sign-off — this is the pragmatic
//   default forced by the API's current shape, not a considered final answer.
// - "Company" is a required field on the backend (inherited from the HR-domain schema this API
//   was merged from) but has no meaningful equivalent in a Doctor/Patient signup — sent as a
//   fixed placeholder rather than adding a UI field for a leftover HR concept.

export const ROLE_OPTIONS: RoleOption[] = [
  { persona: 'doctor', label: 'Doctor' },
  { persona: 'patient', label: 'Patient' }
];

const SIGNUP_COMPANY_PLACEHOLDER = 'N/A';
const STORAGE_KEY = 'hc_auth_session';

interface StoredSession {
  user: AuthUser;
  token: string;
}

interface LoginResponseDto {
  isNotValid: boolean;
  message: string;
  userId: number;
  fullName: string;
  email: string;
  company: string;
  roleId: number;
  token: string;
}

interface UserResponseDto {
  isNotValid: boolean;
  message: string;
  userId: number;
  fullName: string;
  email: string;
  company: string;
}

function personaFromRoleId(roleId: number): Persona {
  return roleId === 1 ? 'doctor' : 'patient';
}

function roleIdFromPersona(persona: Persona): number {
  return persona === 'doctor' ? 1 : 2;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly stored = this.loadFromStorage();
  readonly currentUser = signal<AuthUser | null>(this.stored?.user ?? null);
  readonly isLoggedIn = computed(() => this.currentUser() !== null);

  login(email: string, password: string): Observable<AuthResult> {
    const body = { email: email.trim().toLowerCase(), password };

    return this.http.post<LoginResponseDto>('/api/users/login', body).pipe(
      map((res) => {
        if (res.isNotValid) {
          return { isNotValid: true, message: res.message };
        }
        const user: AuthUser = {
          id: String(res.userId),
          fullName: res.fullName,
          email: res.email,
          persona: personaFromRoleId(res.roleId)
        };
        this.setSession(user, res.token);
        return { isNotValid: false, message: '', user };
      }),
      catchError((err: HttpErrorResponse) => of(this.toAuthResult(err, 'log in')))
    );
  }

  signUp(fullName: string, email: string, password: string, persona: Persona): Observable<AuthResult> {
    const body = {
      fullName,
      email: email.trim().toLowerCase(),
      company: SIGNUP_COMPANY_PLACEHOLDER,
      password,
      roleId: roleIdFromPersona(persona)
    };

    return this.http.post<UserResponseDto>('/api/users/signup', body).pipe(
      map((res) => {
        if (res.isNotValid) {
          return { isNotValid: true, message: res.message };
        }
        return { isNotValid: false, message: 'Account created — you can now sign in.' };
      }),
      catchError((err: HttpErrorResponse) => of(this.toAuthResult(err, 'sign up')))
    );
  }

  logout(): void {
    this.currentUser.set(null);
    localStorage.removeItem(STORAGE_KEY);
  }

  private setSession(user: AuthUser, token: string): void {
    this.currentUser.set(user);
    localStorage.setItem(STORAGE_KEY, JSON.stringify({ user, token } satisfies StoredSession));
  }

  private loadFromStorage(): StoredSession | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) {
      return null;
    }
    try {
      return JSON.parse(raw) as StoredSession;
    } catch {
      return null;
    }
  }

  private toAuthResult(err: HttpErrorResponse, action: string): AuthResult {
    if (err.status === 0) {
      return {
        isNotValid: true,
        message: 'Could not reach the Identity API. Is HC.AI.Identity.Api running on http://localhost:5008?'
      };
    }
    const backendMessage = (err.error as { message?: string } | null)?.message;
    if (backendMessage) {
      return { isNotValid: true, message: backendMessage };
    }
    return { isNotValid: true, message: `Could not ${action} (HTTP ${err.status}).` };
  }
}
