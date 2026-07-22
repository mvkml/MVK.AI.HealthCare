// Demo 3 — signs up a brand-new account through the real UI/backend. Opens a real (headed)
// browser, fills the Sign Up form, submits, and leaves the browser open on the success message.
//
// Run: npm run demo:signup   (from hc_qa/web/aihcweb)

import { chromium } from '@playwright/test';
import crypto from 'crypto';

const BASE_URL = process.env.WEB_BASE_URL ?? 'http://localhost:4200';
const unique = crypto.randomUUID().slice(0, 8);
const FULL_NAME = process.env.DEMO_FULL_NAME ?? 'Demo Signup User';
const EMAIL = process.env.DEMO_EMAIL ?? `demo-signup-${unique}@demo.health`;
const PASSWORD = process.env.DEMO_PASSWORD ?? 'Demo12345';
const PERSONA = process.env.DEMO_PERSONA ?? 'doctor';
// Slows every Playwright action so a human watching can actually follow along.
const SLOW_MO_MS = Number(process.env.DEMO_SLOW_MO_MS ?? 1000);

function banner(title: string) {
  console.log('\n' + '='.repeat(70));
  console.log(title);
  console.log('='.repeat(70));
}

async function main() {
  banner('DEMO 3 — Sign Up');
  console.log(`Web base URL : ${BASE_URL}`);
  console.log(`New account  : ${EMAIL} (${PERSONA})`);

  const browser = await chromium.launch({ headless: false, slowMo: SLOW_MO_MS });
  const page = await browser.newPage();

  banner('STEP 1 — Open the signup page');
  await page.goto(`${BASE_URL}/signup`);

  banner('STEP 2 — Fill in full name and email');
  await page.locator('input[name="fullName"]').pressSequentially(FULL_NAME);
  await page.locator('input[name="email"]').pressSequentially(EMAIL);

  banner('STEP 3 — Select persona');
  await page.locator('select[name="persona"]').selectOption(PERSONA);

  banner('STEP 4 — Enter password and confirm');
  await page.locator('input[name="password"]').pressSequentially(PASSWORD);
  await page.locator('input[name="confirmPassword"]').pressSequentially(PASSWORD);

  banner('STEP 5 — Sign up');
  await page.getByRole('button', { name: /sign up/i }).click();

  banner('DONE — account created');
  console.log('Browser left open for manual use. Press Ctrl+C in this terminal to close it.');

  // Keep the process (and therefore the browser) alive until manually stopped.
  await new Promise(() => {});
}

main().catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
