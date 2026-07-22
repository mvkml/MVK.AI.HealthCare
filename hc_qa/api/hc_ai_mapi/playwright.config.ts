import { defineConfig } from '@playwright/test';

const baseURL = process.env.API_BASE_URL ?? 'http://localhost:5150/api/';

export default defineConfig({
  testDir: './tests',
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  // Each provide-prompt case is a real Ollama generation (7B model) — one at a time, generous
  // timeout. A cold model (not yet loaded into Ollama) has been observed to take ~90s for a
  // single "hi" — 120s leaves headroom above that.
  workers: 1,
  reporter: [['html', { open: 'never' }], ['list']],
  timeout: 120_000,
  use: {
    baseURL,
    extraHTTPHeaders: {
      Accept: 'application/json',
    },
  },
});
