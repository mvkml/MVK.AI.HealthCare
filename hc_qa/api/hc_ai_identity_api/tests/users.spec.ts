import { test, expect } from '@playwright/test';
import { buildUserRequest } from '../fixtures/user';

test.describe('Identity API - Roles', () => {
  test('GET /users/roles returns Doctor and Patient', async ({ request }) => {
    const response = await request.get('users/roles');
    expect(response.status()).toBe(200);

    const body = await response.json();
    expect(Array.isArray(body.roleItems)).toBe(true);

    const names = body.roleItems.map((r: { roleName: string }) => r.roleName);
    expect(names).toEqual(expect.arrayContaining(['Doctor', 'Patient']));
  });
});

test.describe('Identity API - Signup', () => {
  test('POST /users/signup with valid data creates the user', async ({ request }) => {
    const payload = buildUserRequest();
    const response = await request.post('users/signup', { data: payload });
    expect(response.status()).toBe(200);

    const body = await response.json();
    expect(body.isNotValid).toBe(false);
    expect(body.email).toBe(payload.email);
    expect(body.fullName).toBe(payload.fullName);
    expect(body.company).toBe(payload.company);
    expect(body.isActive).toBe(true);
    expect(body.userId).toBeGreaterThan(0);
  });

  test('POST /users/signup without RoleId defaults to Patient (RoleId 2)', async ({ request }) => {
    const payload = buildUserRequest();
    const { roleId, ...withoutRoleId } = payload;
    await request.post('users/signup', { data: withoutRoleId });

    const login = await (
      await request.post('users/login', { data: { email: payload.email, password: payload.password } })
    ).json();
    expect(login.roleId).toBe(2);
  });

  test('POST /users/signup without FullName returns 400', async ({ request }) => {
    const payload = buildUserRequest({ fullName: '' });
    const response = await request.post('users/signup', { data: payload });
    expect(response.status()).toBe(400);

    const body = await response.json();
    expect(body.message).toContain('FullName is required');
  });

  test('POST /users/signup with an invalid email returns 400', async ({ request }) => {
    const payload = buildUserRequest({ email: 'not-an-email' });
    const response = await request.post('users/signup', { data: payload });
    expect(response.status()).toBe(400);

    const body = await response.json();
    expect(body.message).toContain('valid Email');
  });

  test('POST /users/signup without Company returns 400', async ({ request }) => {
    const payload = buildUserRequest({ company: '' });
    const response = await request.post('users/signup', { data: payload });
    expect(response.status()).toBe(400);

    const body = await response.json();
    expect(body.message).toContain('Company is required');
  });

  test('POST /users/signup with a password under 8 characters returns 400', async ({ request }) => {
    const payload = buildUserRequest({ password: 'short1' });
    const response = await request.post('users/signup', { data: payload });
    expect(response.status()).toBe(400);

    const body = await response.json();
    expect(body.message).toContain('at least 8 characters');
  });
});

test.describe('Identity API - Login', () => {
  test('POST /users/login with correct credentials returns a token', async ({ request }) => {
    const user = buildUserRequest();
    await request.post('users/signup', { data: user });

    const response = await request.post('users/login', {
      data: { email: user.email, password: user.password },
    });
    expect(response.status()).toBe(200);

    const body = await response.json();
    expect(body.isNotValid).toBe(false);
    expect(body.email).toBe(user.email);
    expect(typeof body.token).toBe('string');
    expect(body.token.length).toBeGreaterThan(0);
  });

  test('POST /users/login with the wrong password returns 401', async ({ request }) => {
    const user = buildUserRequest();
    await request.post('users/signup', { data: user });

    const response = await request.post('users/login', {
      data: { email: user.email, password: 'WrongPassword1' },
    });
    expect(response.status()).toBe(401);

    const body = await response.json();
    expect(body.message).toContain('Invalid email or password');
  });

  test('POST /users/login for an unknown email returns 401', async ({ request }) => {
    const response = await request.post('users/login', {
      data: { email: `unknown-${Date.now()}@demo.health`, password: 'Whatever123' },
    });
    expect(response.status()).toBe(401);
  });

  test('POST /users/login with an invalid email format returns 400', async ({ request }) => {
    const response = await request.post('users/login', {
      data: { email: 'not-an-email', password: 'Whatever123' },
    });
    expect(response.status()).toBe(400);

    const body = await response.json();
    expect(body.message).toContain('valid Email');
  });

  test('POST /users/login without a password returns 400', async ({ request }) => {
    const user = buildUserRequest();
    await request.post('users/signup', { data: user });

    const response = await request.post('users/login', { data: { email: user.email, password: '' } });
    expect(response.status()).toBe(400);

    const body = await response.json();
    expect(body.message).toContain('Password is required');
  });
});

test.describe('Identity API - Forgot Password', () => {
  test('POST /users/forgot-password for an existing account returns 200', async ({ request }) => {
    const user = buildUserRequest();
    await request.post('users/signup', { data: user });

    const response = await request.post('users/forgot-password', { data: { email: user.email } });
    expect(response.status()).toBe(200);

    const body = await response.json();
    expect(body.isNotValid).toBe(false);
  });

  test('POST /users/forgot-password for an unknown email returns 404', async ({ request }) => {
    const response = await request.post('users/forgot-password', {
      data: { email: `unknown-${Date.now()}@demo.health` },
    });
    expect(response.status()).toBe(404);

    const body = await response.json();
    expect(body.message).toContain('No account found');
  });

  test('POST /users/forgot-password with an invalid email format returns 400', async ({ request }) => {
    const response = await request.post('users/forgot-password', { data: { email: 'not-an-email' } });
    expect(response.status()).toBe(400);
  });
});

test.describe('Identity API - Reset Password', () => {
  test('POST /users/reset-password changes the password, old password stops working', async ({ request }) => {
    const user = buildUserRequest();
    await request.post('users/signup', { data: user });

    const newPassword = 'NewPass12345';
    const response = await request.post('users/reset-password', {
      data: { email: user.email, newPassword },
    });
    expect(response.status()).toBe(200);

    const oldLogin = await request.post('users/login', {
      data: { email: user.email, password: user.password },
    });
    expect(oldLogin.status()).toBe(401);

    const newLogin = await request.post('users/login', {
      data: { email: user.email, password: newPassword },
    });
    expect(newLogin.status()).toBe(200);
  });

  test('POST /users/reset-password for an unknown email returns 404', async ({ request }) => {
    const response = await request.post('users/reset-password', {
      data: { email: `unknown-${Date.now()}@demo.health`, newPassword: 'NewPass12345' },
    });
    expect(response.status()).toBe(404);
  });

  test('POST /users/reset-password with a new password under 8 characters returns 400', async ({ request }) => {
    const user = buildUserRequest();
    await request.post('users/signup', { data: user });

    const response = await request.post('users/reset-password', {
      data: { email: user.email, newPassword: 'short1' },
    });
    expect(response.status()).toBe(400);

    const body = await response.json();
    expect(body.message).toContain('at least 8 characters');
  });

  test('POST /users/reset-password with an invalid email format returns 400', async ({ request }) => {
    const response = await request.post('users/reset-password', {
      data: { email: 'not-an-email', newPassword: 'NewPass12345' },
    });
    expect(response.status()).toBe(400);
  });
});
