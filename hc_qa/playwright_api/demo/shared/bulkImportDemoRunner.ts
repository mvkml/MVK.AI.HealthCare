import fs from 'fs';

export interface BulkImportDemoConfig {
  /** Display name, e.g. "Observation" */
  entityName: string;
  /** REST resource path segment, e.g. "observations" */
  apiResourcePath: string;
  /** Absolute path to the entity's Synthea CSV file */
  csvPath: string;
  /** Module type label passed through to df_chunk_file, used to label the orchestration's results */
  moduleType: string;
  /**
   * Only for entities whose CSV supplies the Id directly (Guid-Id entities, e.g. ClaimTransaction).
   * Omit this for auto-increment entities (no Id column in the CSV) and provide `findRecord` instead.
   */
  idIsInCsv?: boolean;
  /**
   * For auto-increment entities: given the parsed columns of the CSV's first data row and the
   * full GetAll() result set, return the matching record so the final step can display it.
   */
  findRecord?: (firstRowColumns: string[], allRecords: any[]) => any | undefined;
  /** For auto-increment entities: human-readable description of what findRecord is matching on. */
  describeLookup?: (firstRowColumns: string[]) => string;
}

const API_BASE_URL = process.env.API_BASE_URL ?? 'http://localhost:5295/api';
const DF_BASE_URL = process.env.DF_BASE_URL ?? 'http://localhost:7131/api';
const STEP_DELAY_MS = Number(process.env.DEMO_STEP_DELAY_MS ?? 1800);
const POLL_INTERVAL_MS = Number(process.env.DEMO_POLL_INTERVAL_MS ?? 5000);
const MAX_POLLS = Number(process.env.DEMO_MAX_POLLS ?? 60);

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

/**
 * Narrated demo for insert-only entities uploaded through df_chunk_file (the Durable Function
 * chunking client) rather than a direct single-request POST. Used for large files
 * (observations.csv, claims_transactions.csv) that have no natural upsert key, so — unlike
 * runUpsertDemo — this does NOT re-run the import at the end: doing so would create duplicates
 * rather than prove idempotency.
 */
export async function runBulkImportDemo(config: BulkImportDemoConfig): Promise<void> {
  const { entityName, apiResourcePath, csvPath, moduleType } = config;
  const resourceUrl = `${API_BASE_URL}/${apiResourcePath}`;
  const apiEndpoint = `${resourceUrl}/import`;

  banner(`${entityName.toUpperCase()} API DEMO — Bulk Import via df_chunk_file`);
  console.log(`API base URL      : ${API_BASE_URL}`);
  console.log(`df_chunk_file URL : ${DF_BASE_URL}`);
  console.log(`CSV file          : ${csvPath}`);
  console.log(`Target endpoint   : POST ${apiEndpoint} (insert-only, no upsert)`);
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

  let statusQueryUrl = '';
  const documentNumber = `DEMO-${moduleType.toUpperCase()}-${Date.now()}`;
  await step('STEP 3 — Trigger the bulk import via df_chunk_file (StartBulkImport)', async () => {
    const requestBody = {
      documentNumber,
      filePath: csvPath.replace(/\\/g, '/'),
      moduleType,
      apiEndpoint,
    };
    console.log('Request body:');
    console.log(JSON.stringify(requestBody, null, 2));

    const response = await fetch(`${DF_BASE_URL}/StartBulkImport`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(requestBody),
    });
    const startResult = await response.json();
    statusQueryUrl = startResult.statusQueryGetUri ?? startResult.StatusQueryGetUri;

    console.log(`\nResponse status: ${response.status}`);
    console.log(`Orchestration instance: ${startResult.id ?? startResult.Id}`);
    console.log(`Status query URL: ${statusQueryUrl}`);
  });

  let orchestrationOutput: any;
  await step('STEP 4 — Poll the orchestration status until it completes', async () => {
    for (let i = 1; i <= MAX_POLLS; i++) {
      const response = await fetch(statusQueryUrl);
      const statusResult = await response.json();
      const runtimeStatus = statusResult.runtimeStatus;
      console.log(`Poll ${i}: runtimeStatus=${runtimeStatus}`);

      if (runtimeStatus === 'Completed' || runtimeStatus === 'Failed') {
        orchestrationOutput = statusResult.output;
        break;
      }
      await sleep(POLL_INTERVAL_MS);
    }

    console.log('\nOrchestration output:');
    console.log(JSON.stringify(orchestrationOutput, null, 2));
  });

  await step(`STEP 5 — Verify the ${entityName} count after the bulk import`, async () => {
    const response = await fetch(resourceUrl);
    const items = await response.json();
    console.log(`GET /${apiResourcePath} -> ${response.status}`);
    console.log(`Rows before import   : ${beforeCount}`);
    console.log(`Rows after import    : ${items.length}`);
    console.log(
      `Rows reported by df_chunk_file : chunks=${orchestrationOutput?.ChunkCount}, ` +
        `total=${orchestrationOutput?.TotalRows}, inserted=${orchestrationOutput?.InsertedCount}, ` +
        `failed=${orchestrationOutput?.FailedCount}`
    );
  });

  await step('STEP 6 — Fetch one specific record to prove the data landed', async () => {
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
    } else if (config.idIsInCsv) {
      const firstId = firstRowColumns[0];
      console.log(`Looking up Id: ${firstId}`);
      const response = await fetch(`${resourceUrl}/${firstId}`);
      const record = await response.json();
      console.log(`GET /${apiResourcePath}/${firstId} -> ${response.status}`);
      console.log('Record:');
      console.log(JSON.stringify(record, null, 2));
    }
  });

  banner('DEMO COMPLETE');
  console.log(
    `Note: ${entityName} import is insert-only (no natural upsert key) — re-running this demo ` +
      'would create duplicate rows rather than prove idempotency, so this demo does not re-run it.'
  );
}
