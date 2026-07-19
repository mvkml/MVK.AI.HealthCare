import path from 'path';
import { runBulkImportDemo } from '../shared/bulkImportDemoRunner';

const CSV_PATH = process.env.CLAIM_TRANSACTIONS_CSV_PATH ??
  path.resolve(
    __dirname,
    '../../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/claims_transactions.csv'
  );

// claims_transactions.csv column 0 (ID) is a genuine unique Guid supplied directly by the CSV
// (confirmed no duplicates across all 118,466 rows), so the entity uses it as-is rather than a
// server-generated Id.
runBulkImportDemo({
  entityName: 'ClaimTransaction',
  apiResourcePath: 'claimtransactions',
  csvPath: CSV_PATH,
  moduleType: 'ClaimTransaction',
  idIsInCsv: true,
}).catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
