import crypto from 'crypto';

export interface UserRequestPayload {
  fullName: string;
  email: string;
  company: string;
  password: string;
  roleId?: number;
}

/** Builds a valid UserRequest (signup) body with a unique email per call, so repeat runs never collide. */
export function buildUserRequest(overrides: Partial<UserRequestPayload> = {}): UserRequestPayload {
  const unique = crypto.randomUUID().slice(0, 8);
  return {
    fullName: 'Playwright Test User',
    email: `playwright-${unique}@demo.health`,
    company: 'QA Demo Co',
    password: 'Demo12345',
    ...overrides,
  };
}
