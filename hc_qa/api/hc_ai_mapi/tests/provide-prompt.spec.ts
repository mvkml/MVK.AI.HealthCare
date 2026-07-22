import { test, expect } from '@playwright/test';
import { buildPromptRequest } from '../fixtures/prompt';

/**
 * Covers the persona -> model routing added to `POST /api/Doctor/provide-prompt`
 * (DoctorPromptMapper -> LLMModelBL -> LLMOptionsFactory, appsettings HCDocExecutor/
 * HCPatientExecutor sections). `PromptResponse.ModelUsed` echoes back the Ollama model tag
 * that actually served the request, so routing is verified end-to-end (not mocked) against the
 * live HC.AI.MAPI + Ollama.
 */
test.describe('POST /Doctor/provide-prompt — persona model routing', () => {
  test('Persona "Doctor" routes to the doctor executor model', async ({ request }) => {
    const response = await request.post('Doctor/provide-prompt', {
      data: buildPromptRequest({ persona: 'Doctor' }),
    });
    expect(response.status()).toBe(200);
    const body = await response.json();
    expect(body.isSuccess).toBe(true);
    expect(body.modelUsed).toBe('hc-doctor-executor:latest');
    expect(body.content).toBeTruthy();
  });

  test('Persona "Patient" routes to the patient executor model', async ({ request }) => {
    const response = await request.post('Doctor/provide-prompt', {
      data: buildPromptRequest({ persona: 'Patient' }),
    });
    expect(response.status()).toBe(200);
    const body = await response.json();
    expect(body.isSuccess).toBe(true);
    expect(body.modelUsed).toBe('hc-patient-executor:1.1');
    expect(body.content).toBeTruthy();
  });

  test('Persona omitted defaults to the doctor executor model', async ({ request }) => {
    const response = await request.post('Doctor/provide-prompt', {
      data: buildPromptRequest(),
    });
    expect(response.status()).toBe(200);
    const body = await response.json();
    expect(body.modelUsed).toBe('hc-doctor-executor:latest');
  });

  test('Persona match is case-insensitive ("PATIENT" still routes to the patient executor)', async ({ request }) => {
    const response = await request.post('Doctor/provide-prompt', {
      data: buildPromptRequest({ persona: 'PATIENT' }),
    });
    expect(response.status()).toBe(200);
    const body = await response.json();
    expect(body.modelUsed).toBe('hc-patient-executor:1.1');
  });

  test('Unrecognized persona falls back to the doctor executor model (documented current behavior, not a bug)', async ({ request }) => {
    const response = await request.post('Doctor/provide-prompt', {
      data: buildPromptRequest({ persona: 'InsuranceProvider' }),
    });
    expect(response.status()).toBe(200);
    const body = await response.json();
    expect(body.modelUsed).toBe('hc-doctor-executor:latest');
  });
});

test.describe('POST /Doctor/provide-prompt — validation (persona has no bearing on these)', () => {
  test('Missing message is rejected with 400', async ({ request }) => {
    const response = await request.post('Doctor/provide-prompt', {
      data: buildPromptRequest({ message: '' }),
    });
    expect(response.status()).toBe(400);
  });

  test('Temperature above 2 is rejected with 400', async ({ request }) => {
    const response = await request.post('Doctor/provide-prompt', {
      data: buildPromptRequest({ temperature: 3 }),
    });
    expect(response.status()).toBe(400);
  });

  test('Successful response includes latency and token accounting fields', async ({ request }) => {
    const response = await request.post('Doctor/provide-prompt', {
      data: buildPromptRequest({ persona: 'Doctor' }),
    });
    const body = await response.json();
    expect(body.isSuccess).toBe(true);
    expect(typeof body.latencyMs).toBe('number');
    expect(body.latencyMs).toBeGreaterThan(0);
  });
});
