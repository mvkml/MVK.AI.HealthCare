import { test, expect } from '@playwright/test';
import { buildTestUser, seedUser } from '../../../../fixtures/auth';

// /signup is currently a single shared page for Doctor and Patient, differing only by the
// persona dropdown — segregated ahead of time since validation may diverge later (see US017).

test.describe('Signup (Doctor)', () => {
  test('valid signup shows a success message', async ({ page }) => {
    const user = buildTestUser({ persona: 'doctor' });

    await page.goto('/signup');
    await page.locator('input[name="fullName"]').fill(user.fullName);
    await page.locator('input[name="email"]').fill(user.email);
    await page.locator('select[name="persona"]').selectOption('doctor');
    await page.locator('input[name="password"]').fill(user.password);
    await page.locator('input[name="confirmPassword"]').fill(user.password);
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('status')).toContainText('Account created');
  });

  test('mismatched passwords show a client-side error without calling the API', async ({ page }) => {
    const user = buildTestUser({ persona: 'doctor' });

    await page.goto('/signup');
    await page.locator('input[name="fullName"]').fill(user.fullName);
    await page.locator('input[name="email"]').fill(user.email);
    await page.locator('input[name="password"]').fill(user.password);
    await page.locator('input[name="confirmPassword"]').fill('SomethingElse1');
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('alert')).toContainText('Passwords do not match');
  });

  test('password under 8 characters shows a client-side error', async ({ page }) => {
    const user = buildTestUser({ persona: 'doctor', password: 'short1' });

    await page.goto('/signup');
    await page.locator('input[name="fullName"]').fill(user.fullName);
    await page.locator('input[name="email"]').fill(user.email);
    await page.locator('input[name="password"]').fill(user.password);
    await page.locator('input[name="confirmPassword"]').fill(user.password);
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('alert')).toContainText('at least 8 characters');
  });

  test('missing required fields shows a client-side error', async ({ page }) => {
    await page.goto('/signup');
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('alert')).toContainText('Please fill in all fields.');
  });

  test('a whitespace-only full name is treated as missing (client-side error)', async ({ page }) => {
    const user = buildTestUser({ persona: 'doctor' });

    await page.goto('/signup');
    await page.locator('input[name="fullName"]').fill('   ');
    await page.locator('input[name="email"]').fill(user.email);
    await page.locator('input[name="password"]').fill(user.password);
    await page.locator('input[name="confirmPassword"]').fill(user.password);
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('alert')).toContainText('Please fill in all fields.');
  });

  test('an invalid email format is rejected by the backend (no client-side email check)', async ({ page }) => {
    const user = buildTestUser({ persona: 'doctor', email: 'not-an-email' });

    await page.goto('/signup');
    await page.locator('input[name="fullName"]').fill(user.fullName);
    await page.locator('input[name="email"]').fill(user.email);
    await page.locator('input[name="password"]').fill(user.password);
    await page.locator('input[name="confirmPassword"]').fill(user.password);
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('alert')).toContainText('valid Email');
  });

  test('signing up as Doctor lands that account on the Doctor Chat view after login', async ({ page }) => {
    const user = buildTestUser({ persona: 'doctor' });

    await page.goto('/signup');
    await page.locator('input[name="fullName"]').fill(user.fullName);
    await page.locator('input[name="email"]').fill(user.email);
    await page.locator('select[name="persona"]').selectOption('doctor');
    await page.locator('input[name="password"]').fill(user.password);
    await page.locator('input[name="confirmPassword"]').fill(user.password);
    await page.getByRole('button', { name: /sign up/i }).click();
    await expect(page.getByRole('status')).toContainText('Account created');

    await page.goto('/login');
    await page.locator('input[name="email"]').fill(user.email);
    await page.locator('input[name="password"]').fill(user.password);
    await page.getByRole('button', { name: /sign in/i }).click();

    await page.waitForURL(/\/home$/);
    await expect(page.getByRole('heading', { name: 'Doctor Chat' })).toBeVisible();
  });

  test('signing up with an email that already exists is accepted (backend upserts, does not reject)', async ({
    page,
  }) => {
    const user = buildTestUser({ persona: 'doctor' });
    await seedUser(user);

    await page.goto('/signup');
    await page.locator('input[name="fullName"]').fill('Different Name');
    await page.locator('input[name="email"]').fill(user.email);
    await page.locator('input[name="password"]').fill('AnotherPass123');
    await page.locator('input[name="confirmPassword"]').fill('AnotherPass123');
    await page.getByRole('button', { name: /sign up/i }).click();

    await expect(page.getByRole('status')).toContainText('Account created');
  });
});
