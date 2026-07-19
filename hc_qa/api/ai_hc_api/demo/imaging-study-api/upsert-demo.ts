import path from 'path';
import { runUpsertDemo } from '../shared/upsertDemoRunner';

const CSV_PATH = process.env.IMAGING_STUDIES_CSV_PATH ??
  path.resolve(
    __dirname,
    '../../../../hc_apis/az/hc_core_apis/AI.HealthCare.Patient.API/documents/patient_details/synthea/imaging_studies.csv'
  );

// imaging_studies.csv's Id column is Synthea's study Id, not a unique row Id -- one study spans
// many series/instance rows, so it maps to StudyId, not the row's own (auto-increment) primary key.
// Match key: InstanceUid, which is unique per row.
runUpsertDemo({
  entityName: 'ImagingStudy',
  apiResourcePath: 'imagingstudies',
  csvPath: CSV_PATH,
  findRecord: (firstRowColumns, allRecords) => {
    const instanceUid = firstRowColumns[9];
    return allRecords.find((r) => r.instanceUid === instanceUid);
  },
  describeLookup: (firstRowColumns) => `InstanceUid=${firstRowColumns[9]}`,
}).catch((err) => {
  console.error('Demo failed:', err);
  process.exit(1);
});
