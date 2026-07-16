import path from 'path';
import { runUpsertDemo } from '../shared/upsertDemoRunner';

const CSV_PATH = process.env.PROCEDURES_CSV_PATH ??
  path.resolve(
    __dirname,
    '../../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/procedures.csv'
  );

// procedures.csv columns: START,STOP,PATIENT,ENCOUNTER,SYSTEM,CODE,DESCRIPTION,BASE_COST,REASONCODE,REASONDESCRIPTION
// No Id column (server-generated, auto-increment). Match key: PatientId + EncounterId + Code.
runUpsertDemo({
  entityName: 'Procedure',
  apiResourcePath: 'procedures',
  csvPath: CSV_PATH,
  findRecord: (firstRowColumns, allRecords) => {
    const patientId = firstRowColumns[2];
    const encounterId = firstRowColumns[3];
    const code = firstRowColumns[5];
    return allRecords.find((r) => r.patientId === patientId && r.encounterId === encounterId && r.code === code);
  },
  describeLookup: (firstRowColumns) =>
    `PatientId=${firstRowColumns[2]}, EncounterId=${firstRowColumns[3]}, Code=${firstRowColumns[5]}`,
}).catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
