import { test, expect } from '@playwright/test';
import { buildTestUser, seedUser } from '../../../../fixtures/auth';

// /login is currently a single shared page for Doctor and Patient — these cases are identical to
// patient-user/login today. Segregated ahead of time since Doctor/Patient login validation may
// diverge later (see US017).

test.describe('Login (Doctor)', () => {
  test('valid credentials redirect to /home', async ({ page, baseURL }) => {
    const user = buildTestUser({ persona: 'doctor' });
    await seedUser(user);

    await page.goto('/login');
    await page.locator('input[name="email"]').fill(user.email);
    await page.locator('input[name="password"]').fill(user.password);
    await page.getByRole('button', { name: /sign in/i }).click();

    await page.waitForURL(`${baseURL}/home`);
    await expect(page.getByText(user.fullName)).toBeVisible();
  });

  test('wrong password shows an error and stays on /login', async ({ page }) => {
    const user = buildTestUser({ persona: 'doctor' });
    await seedUser(user);

    await page.goto('/login');
    await page.locator('input[name="email"]').fill(user.email);
    await page.locator('input[name="password"]').fill('WrongPassword1');
    await page.getByRole('button', { name: /sign in/i }).click();

    await expect(page.getByRole('alert')).toContainText(/invalid email or password/i);
    await expect(page).toHaveURL(/\/login$/);
  });

  test('unknown email shows an error and stays on /login', async ({ page }) => {
    await page.goto('/login');
    await page.locator('input[name="email"]').fill(`unknown-${Date.now()}@demo.health`);
    await page.locator('input[name="password"]').fill('Whatever123');
    await page.getByRole('button', { name: /sign in/i }).click();

    await expect(page.getByRole('alert')).toContainText(/invalid email or password/i);
  });

  test('empty email/password shows a client-side validation message', async ({ page }) => {
    await page.goto('/login');
    await page.getByRole('button', { name: /sign in/i }).click();

    await expect(page.getByRole('alert')).toContainText('Please enter your email and password.');
  });

  test('unauthenticated visitor to /home is redirected to /login', async ({ page, baseURL }) => {
    await page.goto('/home');
    await page.waitForURL(`${baseURL}/login`);
  });

  test('unauthenticated visitor to /chat is redirected to /login', async ({ page, baseURL }) => {
    await page.goto('/chat');
    await page.waitForURL(`${baseURL}/login`);
  });
});
