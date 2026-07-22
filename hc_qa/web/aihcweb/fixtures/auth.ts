import crypto from 'crypto';

const IDENTITY_API_BASE_URL = process.env.IDENTITY_API_BASE_URL ?? 'http://localhost:5008/api';

export interface TestUser {
  fullName: string;
  email: string;
  password: string;
  persona: 'doctor' | 'patient';
}

/** Builds a test user with a unique email per call, so repeat runs never collide. */
export function buildTestUser(overrides: Partial<TestUser> = {}): TestUser {
  const unique = crypto.randomUUID().slice(0, 8);
  return {
    fullName: 'Playwright QA User',
    email: `playwright-${unique}@demo.health`,
    password: 'Demo12345',
    persona: 'doctor',
    ...overrides,
  };
}

/** Signs up a user directly against HC.AI.Identity.Api (bypassing the UI), so login tests have a known-good account. */
export async function seedUser(user: TestUser): Promise<void> {
  const roleId = user.persona === 'doctor' ? 1 : 2;
  const response = await fetch(`${IDENTITY_API_BASE_URL}/users/signup`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      fullName: user.fullName,
      email: user.email,
      company: 'N/A',
      password: user.password,
      roleId,
    }),
  });
  if (!response.ok) {
    throw new Error(`Failed to seed test user ${user.email}: HTTP ${response.status}`);
  }
}
