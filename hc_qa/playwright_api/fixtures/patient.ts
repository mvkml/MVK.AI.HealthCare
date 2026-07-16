import crypto from 'crypto';

export interface PatientRequestPayload {
  birthDate: string;
  deathDate?: string | null;
  ssn?: string | null;
  drivers?: string | null;
  passport?: string | null;
  prefix?: string | null;
  first: string;
  middle?: string | null;
  last: string;
  suffix?: string | null;
  maiden?: string | null;
  marital?: string | null;
  race?: string | null;
  ethnicity?: string | null;
  gender?: string | null;
  birthplace?: string | null;
  address?: string | null;
  city?: string | null;
  state?: string | null;
  county?: string | null;
  fips?: string | null;
  zip?: string | null;
  lat?: number | null;
  lon?: number | null;
  healthcareExpenses?: number | null;
  healthcareCoverage?: number | null;
  income?: number | null;
}

/** Builds a valid PatientRequest body with a unique last name per call, so parallel/repeat runs never collide. */
export function buildPatientRequest(overrides: Partial<PatientRequestPayload> = {}): PatientRequestPayload {
  const unique = crypto.randomUUID().slice(0, 8);
  return {
    birthDate: '1985-04-12T00:00:00Z',
    ssn: '999-83-4938',
    first: 'Test',
    last: `Playwright-${unique}`,
    gender: 'F',
    address: '123 Test St',
    city: 'Springfield',
    state: 'IL',
    zip: '62704',
    healthcareExpenses: 1234.56,
    healthcareCoverage: 500,
    income: 45000,
    ...overrides,
  };
}

const CSV_HEADER =
  'Id,BIRTHDATE,DEATHDATE,SSN,DRIVERS,PASSPORT,PREFIX,FIRST,MIDDLE,LAST,SUFFIX,MAIDEN,MARITAL,RACE,' +
  'ETHNICITY,GENDER,BIRTHPLACE,ADDRESS,CITY,STATE,COUNTY,FIPS,ZIP,LAT,LON,HEALTHCARE_EXPENSES,' +
  'HEALTHCARE_COVERAGE,INCOME';

export interface PatientCsvRow {
  id: string;
  birthDate?: string;
  deathDate?: string;
  ssn?: string;
  drivers?: string;
  passport?: string;
  prefix?: string;
  first?: string;
  middle?: string;
  last?: string;
  suffix?: string;
  maiden?: string;
  marital?: string;
  race?: string;
  ethnicity?: string;
  gender?: string;
  birthplace?: string;
  address?: string;
  city?: string;
  state?: string;
  county?: string;
  fips?: string;
  zip?: string;
  lat?: string;
  lon?: string;
  healthcareExpenses?: string;
  healthcareCoverage?: string;
  income?: string;
}

/** Builds one Synthea-format patients.csv data row (28 columns) for the given fields. */
export function buildPatientCsvRow(row: PatientCsvRow): string {
  return [
    row.id,
    row.birthDate ?? '1970-01-01',
    row.deathDate ?? '',
    row.ssn ?? '',
    row.drivers ?? '',
    row.passport ?? '',
    row.prefix ?? '',
    row.first ?? 'Csv',
    row.middle ?? '',
    row.last ?? 'Import',
    row.suffix ?? '',
    row.maiden ?? '',
    row.marital ?? '',
    row.race ?? '',
    row.ethnicity ?? '',
    row.gender ?? '',
    row.birthplace ?? '',
    row.address ?? '',
    row.city ?? '',
    row.state ?? '',
    row.county ?? '',
    row.fips ?? '',
    row.zip ?? '',
    row.lat ?? '',
    row.lon ?? '',
    row.healthcareExpenses ?? '',
    row.healthcareCoverage ?? '',
    row.income ?? '',
  ].join(',');
}

/** Builds a full patients.csv file body (header + rows) from a list of row definitions. */
export function buildPatientsCsv(rows: PatientCsvRow[]): string {
  return [CSV_HEADER, ...rows.map(buildPatientCsvRow)].join('\n') + '\n';
}
