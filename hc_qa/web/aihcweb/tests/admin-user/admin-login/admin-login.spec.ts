import { test, expect } from '@playwright/test';
import { buildAdminTestUser, seedAdminAccount } from '../../../fixtures/admin-auth';

test.describe('Admin Login', () => {
  test('valid credentials redirect to /admin/home', async ({ page, baseURL }) => {
    const admin = buildAdminTestUser();

    await page.goto('/admin/login');
    await seedAdminAccount(page, admin);

    await page.locator('input[name="email"]').fill(admin.email);
    await page.locator('input[name="password"]').fill(admin.password);
    await page.getByRole('button', { name: /sign in/i }).click();

    await page.waitForURL(`${baseURL}/admin/home`);
    await expect(page.getByRole('heading', { name: `Welcome, ${admin.fullName}` })).toBeVisible();
  });

  test('wrong password shows an error and stays on /admin/login', async ({ page }) => {
    const admin = buildAdminTestUser();

    await page.goto('/admin/login');
    await seedAdminAccount(page, admin);

    await page.locator('input[name="email"]').fill(admin.email);
    await page.locator('input[name="password"]').fill('WrongPassword1');
    await page.getByRole('button', { name: /sign in/i }).click();

    await expect(page.getByRole('alert')).toContainText('Invalid email or password.');
    await expect(page).toHaveURL(/\/admin\/login$/);
  });

  test('unknown email shows an error', async ({ page }) => {
    await page.goto('/admin/login');
    await page.locator('input[name="email"]').fill(`unknown-admin-${Date.now()}@demo.health`);
    await page.locator('input[name="password"]').fill('Whatever123');
    await page.getByRole('button', { name: /sign in/i }).click();

    await expect(page.getByRole('alert')).toContainText('Invalid email or password.');
  });

  test('empty email/password shows a client-side validation message', async ({ page }) => {
    await page.goto('/admin/login');
    await page.getByRole('button', { name: /sign in/i }).click();

    await expect(page.getByRole('alert')).toContainText('Please enter your email and password.');
  });

  test('unauthenticated visitor to /admin/home is redirected to /admin/login', async ({ page, baseURL }) => {
    await page.goto('/admin/home');
    await page.waitForURL(`${baseURL}/admin/login`);
  });

  test('an admin session does not grant access to the Doctor/Patient area (/home)', async ({ page, baseURL }) => {
    const admin = buildAdminTestUser();

    await page.goto('/admin/login');
    await seedAdminAccount(page, admin);
    await page.locator('input[name="email"]').fill(admin.email);
    await page.locator('input[name="password"]').fill(admin.password);
    await page.getByRole('button', { name: /sign in/i }).click();
    await page.waitForURL(`${baseURL}/admin/home`);

    // Admin login (AdminAuthMockService) and Doctor/Patient login (AuthService) are separate
    // sessions by design (US014: "a separate entry point ... distinct from Doctor/Patient
    // login") — being signed in as Admin must not unlock /home.
    await page.goto('/home');
    await page.waitForURL(`${baseURL}/login`);
  });
});
