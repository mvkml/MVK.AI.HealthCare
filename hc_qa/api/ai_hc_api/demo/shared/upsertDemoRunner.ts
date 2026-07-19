import fs from 'fs';

export interface UpsertDemoConfig {
  /** Display name, e.g. "Patient" */
  entityName: string;
  /** REST resource path segment, e.g. "patients" */
  apiResourcePath: string;
  /** Absolute path to the entity's Synthea CSV file */
  csvPath: string;
  /**
   * Only for entities whose CSV supplies the Id directly (Guid-Id entities). Omit this for
   * auto-increment entities (no Id column in the CSV) and provide `findRecord` instead.
   */
  idIsInCsv?: boolean;
  /**
   * For auto-increment entities: given the parsed columns of the CSV's first data row and the
   * full GetAll() result set, return the matching record so Step 5 can display it.
   */
  findRecord?: (firstRowColumns: string[], allRecords: any[]) => any | undefined;
  /** For auto-increment entities: human-readable description of what findRecord is matching on. */
  describeLookup?: (firstRowColumns: string[]) => string;
}

const API_BASE_URL = process.env.API_BASE_URL ?? 'http://localhost:5295/api';
const STEP_DELAY_MS = Number(process.env.DEMO_STEP_DELAY_MS ?? 1800);

function sleep(ms: number) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function banner(title: string) {
  console.log('\n' + '='.repeat(70));
  console.log(title);
  console.log('='.repeat(70));
}

async function step(title: string, action: () => Promise<void>) {
  banner(title);
  await action();
  await sleep(STEP_DELAY_MS);
}

export async function runUpsertDemo(config: UpsertDemoConfig): Promise<void> {
  const { entityName, apiResourcePath, csvPath } = config;
  const resourceUrl = `${API_BASE_URL}/${apiResourcePath}`;

  banner(`${entityName.toUpperCase()} API DEMO — CSV Upsert`);
  console.log(`API base URL : ${API_BASE_URL}`);
  console.log(`CSV file     : ${csvPath}`);
  await sleep(STEP_DELAY_MS);

  await step('STEP 1 — Read the CSV file', async () => {
    const csvContent = fs.readFileSync(csvPath, 'utf-8');
    const lines = csvContent.trim().split('\n');
    console.log(`Total lines (incl. header): ${lines.length}`);
    console.log(`${entityName} rows: ${lines.length - 1}`);
    console.log('\nHeader:');
    console.log(lines[0]);
    console.log('\nFirst 3 rows:');
    lines.slice(1, 4).forEach((line, i) => console.log(`  [${i + 1}] ${line}`));
  });

  let beforeCount = 0;
  await step(`STEP 2 — Check current ${entityName} count in the database`, async () => {
    const response = await fetch(resourceUrl);
    const items = await response.json();
    beforeCount = items.length;
    console.log(`GET /${apiResourcePath} -> ${response.status}`);
    console.log(`${entityName} rows currently in database: ${beforeCount}`);
  });

  let importResult: any;
  await step(`STEP 3 — Upsert the CSV file via POST /${apiResourcePath}/import/upsert`, async () => {
    const csvContent = fs.readFileSync(csvPath);
    const form = new FormData();
    form.append('file', new Blob([csvContent], { type: 'text/csv' }), csvPath.split(/[\\/]/).pop());

    console.log(`Uploading CSV ...`);
    const response = await fetch(`${resourceUrl}/import/upsert`, {
      method: 'POST',
      body: form,
    });
    importResult = await response.json();

    console.log(`Response status: ${response.status}`);
    console.log('Response body:');
    console.log(JSON.stringify(importResult, null, 2));
  });

  await step(`STEP 4 — Verify the ${entityName} count after upsert`, async () => {
    const response = await fetch(resourceUrl);
    const items = await response.json();
    console.log(`GET /${apiResourcePath} -> ${response.status}`);
    console.log(`Rows before upsert   : ${beforeCount}`);
    console.log(`Rows after upsert    : ${items.length}`);
    console.log(`Rows reported by API : total=${importResult.totalRows}, inserted/upserted=${importResult.insertedCount}, failed=${importResult.failedCount}`);
  });

  await step('STEP 5 — Fetch one specific record to prove the data landed', async () => {
    const csvContent = fs.readFileSync(csvPath, 'utf-8');
    const firstDataLine = csvContent.trim().split('\n')[1];
    const firstRowColumns = firstDataLine.split(',');

    if (config.findRecord) {
      const description = config.describeLookup?.(firstRowColumns) ?? 'matching row';
      console.log(`Looking up: ${description}`);
      const response = await fetch(resourceUrl);
      const allRecords = await response.json();
      const record = config.findRecord(firstRowColumns, allRecords);
      console.log(`GET /${apiResourcePath} -> ${response.status} (${allRecords.length} rows), filtered client-side`);
      console.log('Record:');
      console.log(JSON.stringify(record, null, 2));
    } else {
      const firstId = firstRowColumns[0];
      console.log(`Looking up Id: ${firstId}`);
      const response = await fetch(`${resourceUrl}/${firstId}`);
      const record = await response.json();
      console.log(`GET /${apiResourcePath}/${firstId} -> ${response.status}`);
      console.log('Record:');
      console.log(JSON.stringify(record, null, 2));
    }
  });

  await step('STEP 6 — Re-run the same upsert to prove it is safe to repeat', async () => {
    const csvContent = fs.readFileSync(csvPath);
    const form = new FormData();
    form.append('file', new Blob([csvContent], { type: 'text/csv' }), csvPath.split(/[\\/]/).pop());

    console.log('Uploading the same CSV again ...');
    const response = await fetch(`${resourceUrl}/import/upsert`, {
      method: 'POST',
      body: form,
    });
    const secondResult = await response.json();
    console.log(`Response status: ${response.status}`);
    console.log(JSON.stringify(secondResult, null, 2));

    const countResponse = await fetch(resourceUrl);
    const items = await countResponse.json();
    console.log(`\n${entityName} count after second run: ${items.length} (unchanged -> no duplicates created)`);
  });

  banner('DEMO COMPLETE');
}
