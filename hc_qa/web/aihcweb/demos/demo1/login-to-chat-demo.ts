// Demo 1 — skips the manual "open login page, type email/password, click into chat, type hi,
// click send" routine. Opens a real (headed) browser, logs in as the Doctor demo account, lands
// on the Doctor Chat page, sends "hi", then leaves the browser open so you can keep working in
// it by hand.
//
// Run: npm run demo:login-chat   (from hc_qa/web/aihcweb)

import { chromium } from '@playwright/test';

const BASE_URL = process.env.WEB_BASE_URL ?? 'http://localhost:4200';
const EMAIL = process.env.DEMO_EMAIL ?? 'doctor@demo.health';
const PASSWORD = process.env.DEMO_PASSWORD ?? 'demo1234';
// Slows every Playwright action so a human watching can actually follow along.
const SLOW_MO_MS = Number(process.env.DEMO_SLOW_MO_MS ?? 1000);

function banner(title: string) {
  console.log('\n' + '='.repeat(70));
  console.log(title);
  console.log('='.repeat(70));
}

async function main() {
  banner('DEMO 1 — Login -> Doctor Chat');
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

  banner('STEP 4 — Open Doctor Chat');
  await page.getByRole('link', { name: /open doctor chat/i }).click();
  await page.waitForURL(`${BASE_URL}/chat`);

  banner('STEP 5 — Send "hi"');
  await page.locator('textarea').pressSequentially('hi');
  await page.getByRole('button', { name: /^send$/i }).click();

  banner('DONE — you are on the Doctor Chat page, "hi" sent');
  console.log('Browser left open for manual use. Press Ctrl+C in this terminal to close it.');

  // Keep the process (and therefore the browser) alive until manually stopped.
  await new Promise(() => {});
}

main().catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
