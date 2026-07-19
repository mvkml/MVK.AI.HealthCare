import path from 'path';
import { runUpsertDemo } from '../shared/upsertDemoRunner';

const CSV_PATH = process.env.PAYERS_CSV_PATH ??
  path.resolve(
    __dirname,
    '../../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/payers.csv'
  );

runUpsertDemo({
  entityName: 'Payer',
  apiResourcePath: 'payers',
  csvPath: CSV_PATH,
}).catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
