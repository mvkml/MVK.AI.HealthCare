-- Phase 1 Doctor-persona allow-list, scoped to the domain map already locked in
-- healthcare_ai_assistant_mcp_ollama_design.md #7 (Patient Identity + Hospital/Clinical Care only
-- -- Insurance/Billing stays out of scope until a later persona phase).
--
-- Table names here are the REAL table names in AI_HealthCarePatient (EF Core's default
-- pluralizing convention), verified against INFORMATION_SCHEMA.TABLES -- NOT the singular
-- entity class names in AI.HealthCare.Patient.EF.Entities.

INSERT INTO dbo.TableWhitelist (Persona, TableName, IsRoot) VALUES
('Doctor', 'Patients',      1),
('Doctor', 'Encounters',    1),
('Doctor', 'Conditions',    0),
('Doctor', 'Providers',     0),
('Doctor', 'Organizations', 0);
GO

INSERT INTO dbo.ColumnWhitelist (TableName, ColumnName) VALUES
('Patients', 'Id'), ('Patients', 'First'), ('Patients', 'Last'), ('Patients', 'BirthDate'),
('Patients', 'Gender'), ('Patients', 'City'), ('Patients', 'State'),
('Encounters', 'Id'), ('Encounters', 'Start'), ('Encounters', 'Stop'), ('Encounters', 'PatientId'),
('Encounters', 'ProviderId'), ('Encounters', 'OrganizationId'), ('Encounters', 'EncounterClass'),
('Encounters', 'Description'), ('Encounters', 'ReasonDescription'),
('Conditions', 'Id'), ('Conditions', 'PatientId'), ('Conditions', 'EncounterId'),
('Conditions', 'Description'), ('Conditions', 'Start'), ('Conditions', 'Stop'),
('Providers', 'Id'), ('Providers', 'Name'), ('Providers', 'Speciality'),
('Organizations', 'Id'), ('Organizations', 'Name'), ('Organizations', 'City');
GO

-- Join pairs mirror the real FK columns on the EF entities (AI.HealthCare.Patient.EF.Entities) --
-- the join condition itself is always resolved from this table, never from caller input.
--
-- NOTE: 'Conditions' is a child table (FK column lives on Conditions, not on the tables that
-- would need to reach it) -- 'Conditions' -> 'Patients'/'Encounters' below only fire if
-- Conditions is ever made IsRoot=1, which it currently isn't. The 'Encounters' -> 'Conditions'
-- row is the one that actually makes Conditions reachable today (root=Encounters, join=Conditions,
-- joined the reverse way: Encounters.Id = Conditions.EncounterId).
INSERT INTO dbo.JoinWhitelist (Persona, FromTable, ToTable, FromColumn, ToColumn) VALUES
('Doctor', 'Encounters', 'Patients',      'PatientId',      'Id'),
('Doctor', 'Encounters', 'Providers',     'ProviderId',     'Id'),
('Doctor', 'Encounters', 'Organizations', 'OrganizationId', 'Id'),
('Doctor', 'Encounters', 'Conditions',    'Id',              'EncounterId'),
('Doctor', 'Conditions', 'Patients',      'PatientId',      'Id'),
('Doctor', 'Conditions', 'Encounters',    'EncounterId',    'Id');
GO
