// Demo 2 — same as demo1, but the Patient demo account. Logs in, lands on the "My Health
// Assistant" page, and leaves the browser open so you can keep working in it by hand.
//
// Run: npm run demo:login-patient-chat   (from hc_qa/web/aihcweb)

import { chromium } from '@playwright/test';

const BASE_URL = process.env.WEB_BASE_URL ?? 'http://localhost:4200';
const EMAIL = process.env.DEMO_EMAIL ?? 'patient@demo.health';
const PASSWORD = process.env.DEMO_PASSWORD ?? 'demo1234';
// Slows every Playwright action so a human watching can actually follow along.
const SLOW_MO_MS = Number(process.env.DEMO_SLOW_MO_MS ?? 1000);

function banner(title: string) {
  console.log('\n' + '='.repeat(70));
  console.log(title);
  console.log('='.repeat(70));
}

async function main() {
  banner('DEMO 2 — Login -> My Health Assistant (Patient)');
  console.log(`Web base URL : ${BASE_URL}`);
  console.log(`Account      : ${EMAIL}`);

  const browser = await chromium.launch({ headless: false, slowMo: SLOW_MO_MS });
  const page = await browser.newPage();

  banner('STEP 1 — Open the login page');
  await page.goto(`${BASE_URL}/login`);

  banner('STEP 2 — Enter email and password');
  await page.locator('input[name="email"]').pressSequentially(EMAIL);
  await page.locator('input[name="password"]').pressSequentially(PASSWORD);

  banner('STEP 3 — Sign in');
  await page.getByRole('button', { name: /sign in/i }).click();
  await page.waitForURL(`${BASE_URL}/home`);

  banner('STEP 4 — Open My Health Assistant');
  await page.getByRole('link', { name: /open my health assistant/i }).click();
  await page.waitForURL(`${BASE_URL}/patient-chat`);

  banner('DONE — you are on the My Health Assistant page');
  console.log('Browser left open for manual use. Press Ctrl+C in this terminal to close it.');

  // Keep the process (and therefore the browser) alive until manually stopped.
  await new Promise(() => {});
}

main().catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
