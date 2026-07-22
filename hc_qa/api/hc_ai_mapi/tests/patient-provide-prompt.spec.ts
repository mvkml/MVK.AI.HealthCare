import { test, expect } from '@playwright/test';
import { buildPromptRequest } from '../fixtures/prompt';

/**
 * `POST /api/Patient/provide-prompt` (PatientController -> PatientService -> PatientPromptMapper
 * -> PatientSemanticProcess) — the real backend behind the Angular Patient chat page, which
 * dropped `PatientChatMockService` in favor of `PatientChatService` calling this endpoint.
 *
 * Unlike `DoctorController` (whose `DoctorPromptMapper` reads `request.Persona` from the client —
 * see `hc_qa/api/hc_ai_mapi/tests/provide-prompt.spec.ts` case 5, the still-open US021 gap),
 * `PatientPromptMapper` hardcodes `PatientPersonaName`/`PatientExecutorPersonaName` outright and
 * never reads `request.Persona` at all. The "ignores client-supplied persona" case below verifies
 * that directly.
 */
test.describe('POST /Patient/provide-prompt', () => {
  test('Valid message routes to the patient executor model', async ({ request }) => {
    const response = await request.post('Patient/provide-prompt', {
      data: buildPromptRequest({ persona: 'Patient' }),
    });
    expect(response.status()).toBe(200);
    const body = await response.json();
    expect(body.isSuccess).toBe(true);
    expect(body.modelUsed).toBe('hc-patient-executor:1.1');
    expect(body.content).toBeTruthy();
  });

  test('Client-supplied persona is ignored — always routes to the patient executor', async ({ request }) => {
    // Deliberately sends "Doctor" to prove the endpoint decides its own persona rather than
    // trusting the request body (PatientPromptMapper's doc-comment claim).
    const response = await request.post('Patient/provide-prompt', {
      data: buildPromptRequest({ persona: 'Doctor' }),
    });
    expect(response.status()).toBe(200);
    const body = await response.json();
    expect(body.modelUsed).toBe('hc-patient-executor:1.1');
  });

  test('Persona omitted still routes to the patient executor', async ({ request }) => {
    const response = await request.post('Patient/provide-prompt', {
      data: buildPromptRequest(),
    });
    expect(response.status()).toBe(200);
    const body = await response.json();
    expect(body.modelUsed).toBe('hc-patient-executor:1.1');
  });

  test('Missing message is rejected with 400', async ({ request }) => {
    const response = await request.post('Patient/provide-prompt', {
      data: buildPromptRequest({ message: '' }),
    });
    expect(response.status()).toBe(400);
  });

  test('Temperature above 2 is rejected with 400', async ({ request }) => {
    const response = await request.post('Patient/provide-prompt', {
      data: buildPromptRequest({ temperature: 3 }),
    });
    expect(response.status()).toBe(400);
  });

  test('Successful response includes latency and model fields', async ({ request }) => {
    const response = await request.post('Patient/provide-prompt', {
      data: buildPromptRequest(),
    });
    const body = await response.json();
    expect(body.isSuccess).toBe(true);
    expect(typeof body.latencyMs).toBe('number');
    expect(body.latencyMs).toBeGreaterThan(0);
  });
});
