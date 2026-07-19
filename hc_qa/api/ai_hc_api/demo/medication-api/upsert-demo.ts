import path from 'path';
import { runUpsertDemo } from '../shared/upsertDemoRunner';

const CSV_PATH = process.env.MEDICATIONS_CSV_PATH ??
  path.resolve(
    __dirname,
    '../../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/medications.csv'
  );

// medications.csv columns: START,STOP,PATIENT,PAYER,ENCOUNTER,CODE,DESCRIPTION,BASE_COST,
// PAYER_COVERAGE,DISPENSES,TOTALCOST,REASONCODE,REASONDESCRIPTION
// No Id column (server-generated, auto-increment). Match key: PatientId + EncounterId + Code.
runUpsertDemo({
  entityName: 'Medication',
  apiResourcePath: 'medications',
  csvPath: CSV_PATH,
  findRecord: (firstRowColumns, allRecords) => {
    const patientId = firstRowColumns[2];
    const encounterId = firstRowColumns[4];
    const code = firstRowColumns[5];
    return allRecords.find((r) => r.patientId === patientId && r.encounterId === encounterId && r.code === code);
  },
  describeLookup: (firstRowColumns) =>
    `PatientId=${firstRowColumns[2]}, EncounterId=${firstRowColumns[4]}, Code=${firstRowColumns[5]}`,
}).catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
