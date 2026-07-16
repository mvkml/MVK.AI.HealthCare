import path from 'path';
import { runUpsertDemo } from '../shared/upsertDemoRunner';

const CSV_PATH = process.env.IMMUNIZATIONS_CSV_PATH ??
  path.resolve(
    __dirname,
    '../../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/immunizations.csv'
  );

// immunizations.csv columns: DATE,PATIENT,ENCOUNTER,CODE,DESCRIPTION,BASE_COST
// No Id column (server-generated, auto-increment). Match key: PatientId + EncounterId + Code.
runUpsertDemo({
  entityName: 'Immunization',
  apiResourcePath: 'immunizations',
  csvPath: CSV_PATH,
  findRecord: (firstRowColumns, allRecords) => {
    const patientId = firstRowColumns[1];
    const encounterId = firstRowColumns[2];
    const code = firstRowColumns[3];
    return allRecords.find((r) => r.patientId === patientId && r.encounterId === encounterId && r.code === code);
  },
  describeLookup: (firstRowColumns) =>
    `PatientId=${firstRowColumns[1]}, EncounterId=${firstRowColumns[2]}, Code=${firstRowColumns[3]}`,
}).catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
