import path from 'path';
import { runUpsertDemo } from '../shared/upsertDemoRunner';

const CSV_PATH = process.env.ORGANIZATIONS_CSV_PATH ??
  path.resolve(
    __dirname,
    '../../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/organizations.csv'
  );

runUpsertDemo({
  entityName: 'Organization',
  apiResourcePath: 'organizations',
  csvPath: CSV_PATH,
}).catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
