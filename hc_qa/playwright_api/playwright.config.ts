import { defineConfig } from '@playwright/test';

// Trailing slash matters: WHATWG URL resolution drops the /api path segment
// if a request path starts with '/' and baseURL has no trailing slash.
const baseURL = process.env.API_BASE_URL ?? 'http://localhost:5295/api/';

export default defineConfig({
  testDir: './tests',
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  workers: 1,
  reporter: [['html', { open: 'never' }], ['list']],
  timeout: 30_000,
  use: {
    baseURL,
    extraHTTPHeaders: {
      Accept: 'application/json',
    },
  },
});
