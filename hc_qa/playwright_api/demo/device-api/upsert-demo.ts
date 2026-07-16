import path from 'path';
import { runUpsertDemo } from '../shared/upsertDemoRunner';

const CSV_PATH = process.env.DEVICES_CSV_PATH ??
  path.resolve(
    __dirname,
    '../../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/devices.csv'
  );

// devices.csv columns: START,STOP,PATIENT,ENCOUNTER,CODE,DESCRIPTION,UDI
// No Id column (server-generated, auto-increment). Match key: PatientId + EncounterId + Udi.
runUpsertDemo({
  entityName: 'Device',
  apiResourcePath: 'devices',
  csvPath: CSV_PATH,
  findRecord: (firstRowColumns, allRecords) => {
    const patientId = firstRowColumns[2];
    const encounterId = firstRowColumns[3];
    const udi = firstRowColumns[6];
    return allRecords.find((r) => r.patientId === patientId && r.encounterId === encounterId && r.udi === udi);
  },
  describeLookup: (firstRowColumns) =>
    `PatientId=${firstRowColumns[2]}, EncounterId=${firstRowColumns[3]}, Udi=${firstRowColumns[6]}`,
}).catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
