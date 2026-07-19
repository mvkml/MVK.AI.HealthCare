import { test, expect, APIRequestContext } from '@playwright/test';
import crypto from 'crypto';
import { buildPatientRequest, buildPatientsCsv } from '../fixtures/patient';

async function deletePatient(request: APIRequestContext, id: string) {
  await request.delete(`patients/${id}`);
}

test.describe('Patients API - CRUD', () => {
  const createdIds: string[] = [];

  test.afterAll(async ({ request }) => {
    for (const id of createdIds) {
      await deletePatient(request, id);
    }
  });

  test('POST /patients creates a patient', async ({ request }) => {
    const payload = buildPatientRequest();
    const response = await request.post('patients', { data: payload });
    expect(response.status()).toBe(200);

    const body = await response.json();
    createdIds.push(body.id);

    expect(body.id).toMatch(/^[0-9a-f-]{36}$/i);
    expect(body.first).toBe(payload.first);
    expect(body.last).toBe(payload.last);
    expect(body.ssn).toBeNull(); // PII masked/omitted unless includePii=true
  });

  test('POST /patients?includePii=true returns masked SSN', async ({ request }) => {
    const payload = buildPatientRequest({ ssn: '999-83-4938' });
    const response = await request.post('patients?includePii=true', { data: payload });
    expect(response.status()).toBe(200);

    const body = await response.json();
    createdIds.push(body.id);

    expect(body.ssn).toBe('***-**-4938');
  });

  test('POST /patients without First name returns 400', async ({ request }) => {
    const payload = buildPatientRequest({ first: '' });
    const response = await request.post('patients', { data: payload });
    expect(response.status()).toBe(400);

    const body = await response.json();
    expect(body.message).toContain('First name is required');
  });

  test('POST /patients with future BirthDate returns 400', async ({ request }) => {
    const payload = buildPatientRequest({ birthDate: '2999-01-01T00:00:00Z' });
    const response = await request.post('patients', { data: payload });
    expect(response.status()).toBe(400);

    const body = await response.json();
    expect(body.message).toContain('valid BirthDate');
  });

  test('GET /patients returns a list including a created patient', async ({ request }) => {
    const payload = buildPatientRequest();
    const created = await (await request.post('patients', { data: payload })).json();
    createdIds.push(created.id);

    const response = await request.get('patients');
    expect(response.status()).toBe(200);

    const body = await response.json();
    expect(Array.isArray(body)).toBe(true);
    expect(body.some((p: { id: string }) => p.id === created.id)).toBe(true);
  });

  test('GET /patients/{id} returns the created patient', async ({ request }) => {
    const payload = buildPatientRequest();
    const created = await (await request.post('patients', { data: payload })).json();
    createdIds.push(created.id);

    const response = await request.get(`patients/${created.id}`);
    expect(response.status()).toBe(200);

    const body = await response.json();
    expect(body.id).toBe(created.id);
    expect(body.last).toBe(payload.last);
  });

  test('GET /patients/{id} with unknown id returns 404', async ({ request }) => {
    const response = await request.get(`patients/${crypto.randomUUID()}`);
    expect(response.status()).toBe(404);
  });

  test('PUT /patients/{id} updates an existing patient', async ({ request }) => {
    const payload = buildPatientRequest();
    const created = await (await request.post('patients', { data: payload })).json();
    createdIds.push(created.id);

    const updatedPayload = buildPatientRequest({ city: 'Chicago', income: 99000 });
    const response = await request.put(`patients/${created.id}`, { data: updatedPayload });
    expect(response.status()).toBe(200);

    const body = await response.json();
    expect(body.id).toBe(created.id);
    expect(body.city).toBe('Chicago');
    expect(body.income).toBe(99000);

    const fetched = await (await request.get(`patients/${created.id}`)).json();
    expect(fetched.city).toBe('Chicago');
  });

  test('PUT /patients/{id} with unknown id returns 404', async ({ request }) => {
    const response = await request.put(`patients/${crypto.randomUUID()}`, {
      data: buildPatientRequest(),
    });
    expect(response.status()).toBe(404);
  });

  test('DELETE /patients/{id} removes the patient', async ({ request }) => {
    const payload = buildPatientRequest();
    const created = await (await request.post('patients', { data: payload })).json();

    const response = await request.delete(`patients/${created.id}`);
    expect(response.status()).toBe(200);

    const fetched = await request.get(`patients/${created.id}`);
    expect(fetched.status()).toBe(404);
  });

  test('DELETE /patients/{id} with unknown id returns 404', async ({ request }) => {
    const response = await request.delete(`patients/${crypto.randomUUID()}`);
    expect(response.status()).toBe(404);
  });
});

test.describe('Patients API - CSV import', () => {
  const createdIds: string[] = [];

  test.afterAll(async ({ request }) => {
    for (const id of createdIds) {
      await deletePatient(request, id);
    }
  });

  test('POST /patients/import inserts new rows from CSV', async ({ request }) => {
    const idA = crypto.randomUUID();
    const idB = crypto.randomUUID();
    createdIds.push(idA, idB);

    const csv = buildPatientsCsv([
      { id: idA, first: 'ImportA', last: 'RowA', city: 'Austin' },
      { id: idB, first: 'ImportB', last: 'RowB', city: 'Dallas' },
    ]);

    const response = await request.post('patients/import', {
      multipart: { file: { name: 'patients.csv', mimeType: 'text/csv', buffer: Buffer.from(csv) } },
    });
    expect(response.status()).toBe(200);

    const result = await response.json();
    expect(result.totalRows).toBe(2);
    expect(result.insertedCount).toBe(2);
    expect(result.failedCount).toBe(0);

    const fetched = await (await request.get(`patients/${idA}`)).json();
    expect(fetched.first).toBe('ImportA');
    expect(fetched.city).toBe('Austin');
  });

  test('POST /patients/import reports malformed rows as failures', async ({ request }) => {
    const idGood = crypto.randomUUID();
    createdIds.push(idGood);

    const goodRow = buildPatientsCsv([{ id: idGood, first: 'Good', last: 'Row' }]).split('\n')[1];
    const badRow = 'not-a-guid,bad-date'; // far too few columns, will throw in the mapper
    const csv = ['Id,BIRTHDATE,DEATHDATE,SSN,DRIVERS,PASSPORT,PREFIX,FIRST,MIDDLE,LAST,SUFFIX,MAIDEN,MARITAL,RACE,ETHNICITY,GENDER,BIRTHPLACE,ADDRESS,CITY,STATE,COUNTY,FIPS,ZIP,LAT,LON,HEALTHCARE_EXPENSES,HEALTHCARE_COVERAGE,INCOME', goodRow, badRow].join('\n') + '\n';

    const response = await request.post('patients/import', {
      multipart: { file: { name: 'patients.csv', mimeType: 'text/csv', buffer: Buffer.from(csv) } },
    });
    expect(response.status()).toBe(200);

    const result = await response.json();
    expect(result.totalRows).toBe(2);
    expect(result.insertedCount).toBe(1);
    expect(result.failedCount).toBe(1);
    expect(result.errors.length).toBe(1);
  });

  test('POST /patients/import rejects a non-CSV file', async ({ request }) => {
    const response = await request.post('patients/import', {
      multipart: { file: { name: 'patients.txt', mimeType: 'text/plain', buffer: Buffer.from('not a csv') } },
    });
    expect(response.status()).toBe(400);
  });

  test('POST /patients/import rejects an empty file', async ({ request }) => {
    const response = await request.post('patients/import', {
      multipart: { file: { name: 'patients.csv', mimeType: 'text/csv', buffer: Buffer.from('') } },
    });
    expect(response.status()).toBe(400);
  });
});

test.describe('Patients API - CSV import/upsert', () => {
  const createdIds: string[] = [];

  test.afterAll(async ({ request }) => {
    for (const id of createdIds) {
      await deletePatient(request, id);
    }
  });

  test('POST /patients/import/upsert inserts rows that do not yet exist', async ({ request }) => {
    const id = crypto.randomUUID();
    createdIds.push(id);

    const csv = buildPatientsCsv([{ id, first: 'UpsertNew', last: 'Row', city: 'Denver' }]);
    const response = await request.post('patients/import/upsert', {
      multipart: { file: { name: 'patients.csv', mimeType: 'text/csv', buffer: Buffer.from(csv) } },
    });
    expect(response.status()).toBe(200);

    const result = await response.json();
    expect(result.insertedCount).toBe(1);
    expect(result.failedCount).toBe(0);

    const fetched = await (await request.get(`patients/${id}`)).json();
    expect(fetched.city).toBe('Denver');
  });

  test('POST /patients/import/upsert updates rows whose Id already exists, without duplicating', async ({ request }) => {
    const id = crypto.randomUUID();
    createdIds.push(id);

    const firstCsv = buildPatientsCsv([{ id, first: 'Original', last: 'Row', city: 'Denver', income: '10000' }]);
    await request.post('patients/import/upsert', {
      multipart: { file: { name: 'patients.csv', mimeType: 'text/csv', buffer: Buffer.from(firstCsv) } },
    });

    const secondCsv = buildPatientsCsv([{ id, first: 'Updated', last: 'Row', city: 'Boulder', income: '20000' }]);
    const response = await request.post('patients/import/upsert', {
      multipart: { file: { name: 'patients.csv', mimeType: 'text/csv', buffer: Buffer.from(secondCsv) } },
    });
    expect(response.status()).toBe(200);

    const fetched = await (await request.get(`patients/${id}`)).json();
    expect(fetched.first).toBe('Updated');
    expect(fetched.city).toBe('Boulder');
    expect(fetched.income).toBe(20000);

    const all = await (await request.get('patients')).json();
    const matches = all.filter((p: { id: string }) => p.id === id);
    expect(matches.length).toBe(1); // confirms update-in-place, not a duplicate insert
  });
});
