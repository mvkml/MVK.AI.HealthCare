import { Page } from '@playwright/test';
import crypto from 'crypto';

const ADMIN_ACCOUNTS_KEY = 'hc_admin_accounts';

export interface AdminTestUser {
  fullName: string;
  email: string;
  password: string;
}

/** Builds an admin test user with a unique email per call, so repeat runs never collide. */
export function buildAdminTestUser(overrides: Partial<AdminTestUser> = {}): AdminTestUser {
  const unique = crypto.randomUUID().slice(0, 8);
  return {
    fullName: 'Playwright Admin User',
    email: `playwright-admin-${unique}@demo.health`,
    password: 'AdminPass123',
    ...overrides,
  };
}

/**
 * Seeds an admin account directly into localStorage, matching AdminAuthMockService's storage
 * shape (`hc_admin_accounts`). AdminAuthMockService is mock-only (no backend Admin role exists
 * yet — see US014), so there's no API to seed against; this bypasses the signup UI the same way
 * `seedUser()` bypasses the real signup UI for the backed-by-a-real-API accounts.
 * Call after `page.goto()` so the page's origin (and therefore localStorage) is available.
 */
export async function seedAdminAccount(page: Page, user: AdminTestUser): Promise<void> {
  // Id generated here (Node side), not inside the evaluate callback below — a bare
  // `crypto.randomUUID()` call there resolves to this file's Node `crypto` import (captured by
  // closure during bundling), not the browser's global `crypto`, and throws in-page.
  const id = crypto.randomUUID();
  await page.evaluate(
    ({ key, account }) => {
      const raw = window.localStorage.getItem(key);
      const accounts = raw ? JSON.parse(raw) : [];
      accounts.push(account);
      window.localStorage.setItem(key, JSON.stringify(accounts));
    },
    { key: ADMIN_ACCOUNTS_KEY, account: { id, ...user } }
  );
}
