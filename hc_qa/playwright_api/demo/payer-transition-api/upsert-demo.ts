import path from 'path';
import { runUpsertDemo } from '../shared/upsertDemoRunner';

const CSV_PATH = process.env.PAYER_TRANSITIONS_CSV_PATH ??
  path.resolve(
    __dirname,
    '../../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/payer_transitions.csv'
  );

// payer_transitions.csv has no Id column (server-generated, auto-increment). Match key: PatientId + MemberId.
runUpsertDemo({
  entityName: 'PayerTransition',
  apiResourcePath: 'payertransitions',
  csvPath: CSV_PATH,
  findRecord: (firstRowColumns, allRecords) => {
    const [patientId, memberId] = firstRowColumns;
    return allRecords.find((r) => r.patientId === patientId && r.memberId === memberId);
  },
  describeLookup: (firstRowColumns) => `PatientId=${firstRowColumns[0]}, MemberId=${firstRowColumns[1]}`,
}).catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
