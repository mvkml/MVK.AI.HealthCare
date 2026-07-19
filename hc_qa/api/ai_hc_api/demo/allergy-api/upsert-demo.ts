import path from 'path';
import { runUpsertDemo } from '../shared/upsertDemoRunner';

const CSV_PATH = process.env.ALLERGIES_CSV_PATH ??
  path.resolve(
    __dirname,
    '../../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/allergies.csv'
  );

// allergies.csv columns: START,STOP,PATIENT,ENCOUNTER,CODE,SYSTEM,DESCRIPTION,TYPE,CATEGORY,
// REACTION1,DESCRIPTION1,SEVERITY1,REACTION2,DESCRIPTION2,SEVERITY2
// No Id column (server-generated, auto-increment). Match key: PatientId + EncounterId + Code.
runUpsertDemo({
  entityName: 'Allergy',
  apiResourcePath: 'allergies',
  csvPath: CSV_PATH,
  findRecord: (firstRowColumns, allRecords) => {
    const patientId = firstRowColumns[2];
    const encounterId = firstRowColumns[3];
    const code = firstRowColumns[4];
    return allRecords.find((r) => r.patientId === patientId && r.encounterId === encounterId && r.code === code);
  },
  describeLookup: (firstRowColumns) =>
    `PatientId=${firstRowColumns[2]}, EncounterId=${firstRowColumns[3]}, Code=${firstRowColumns[4]}`,
}).catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
