import path from 'path';
import { runBulkImportDemo } from '../shared/bulkImportDemoRunner';

const CSV_PATH = process.env.OBSERVATIONS_CSV_PATH ??
  path.resolve(
    __dirname,
    '../../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/observations.csv'
  );

// observations.csv columns: DATE,PATIENT,ENCOUNTER,CATEGORY,CODE,DESCRIPTION,VALUE,UNITS,TYPE
// No Id column (server-generated, auto-increment). Match key: PatientId + EncounterId + Code.
// Note: ~2,994 rows (yearly DALY/QALY/QOLS summary scores) have no ENCOUNTER at all —
// EncounterId is nullable on this entity to allow for that.
runBulkImportDemo({
  entityName: 'Observation',
  apiResourcePath: 'observations',
  csvPath: CSV_PATH,
  moduleType: 'Observation',
  findRecord: (firstRowColumns, allRecords) => {
    const patientId = firstRowColumns[1];
    const encounterId = firstRowColumns[2];
    const code = firstRowColumns[4];
    return allRecords.find((r) => r.patientId === patientId && r.encounterId === encounterId && r.code === code);
  },
  describeLookup: (firstRowColumns) =>
    `PatientId=${firstRowColumns[1]}, EncounterId=${firstRowColumns[2]}, Code=${firstRowColumns[4]}`,
}).catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
