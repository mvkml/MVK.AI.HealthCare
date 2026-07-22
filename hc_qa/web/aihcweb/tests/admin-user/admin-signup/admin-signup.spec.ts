import { test, expect } from '@playwright/test';
import { buildAdminTestUser, seedAdminAccount } from '../../../fixtures/admin-auth';

test.describe('Admin Signup', () => {
  test('valid signup shows a success message', async ({ page }) => {
    const admin = buildAdminTestUser();

    await page.goto('/admin/signup');
    await page.locator('input[name="fullName"]').fill(admin.fullName);
    await page.locator('input[name="email"]').fill(admin.email);
    await page.locator('input[name="password"]').fill(admin.password);
    await page.locator('input[name="confirmPassword"]').fill(admin.password);
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('status')).toContainText('Admin account created');
  });

  test('mismatched passwords show a client-side error', async ({ page }) => {
    const admin = buildAdminTestUser();

    await page.goto('/admin/signup');
    await page.locator('input[name="fullName"]').fill(admin.fullName);
    await page.locator('input[name="email"]').fill(admin.email);
    await page.locator('input[name="password"]').fill(admin.password);
    await page.locator('input[name="confirmPassword"]').fill('SomethingElse1');
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('alert')).toContainText('Passwords do not match');
  });

  test('password under 8 characters shows a client-side error', async ({ page }) => {
    const admin = buildAdminTestUser({ password: 'short1' });

    await page.goto('/admin/signup');
    await page.locator('input[name="fullName"]').fill(admin.fullName);
    await page.locator('input[name="email"]').fill(admin.email);
    await page.locator('input[name="password"]').fill(admin.password);
    await page.locator('input[name="confirmPassword"]').fill(admin.password);
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('alert')).toContainText('at least 8 characters');
  });

  test('missing required fields shows a client-side error', async ({ page }) => {
    await page.goto('/admin/signup');
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('alert')).toContainText('Please fill in all fields.');
  });

  test('signing up with an email that already exists is rejected (unlike the real Identity API, this mock does not upsert)', async ({
    page,
  }) => {
    const admin = buildAdminTestUser();

    await page.goto('/admin/signup');
    await seedAdminAccount(page, admin);

    await page.locator('input[name="fullName"]').fill('Different Name');
    await page.locator('input[name="email"]').fill(admin.email);
    await page.locator('input[name="password"]').fill('AnotherPass123');
    await page.locator('input[name="confirmPassword"]').fill('AnotherPass123');
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('alert')).toContainText('An admin account with that email already exists.');
  });
});
