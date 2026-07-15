# Product Roadmap

High-level plan for the mvkhc project.

## Phase 1 - Foundation & Data (Sprint 1)
- Explore and profile the Synthea synthetic patient dataset (PySpark)
- Clean up leftover HR-project artifacts in hc_bigdata
- Document entity relationships to inform API design

## Phase 2 - Core APIs (Sprint 2)
- Build AI.HealthCare.Patient.API (patient records)
- Build AI.HC.Api (core/shared healthcare capabilities)
- Design SQL schema / data source layer (hc_data_source)

## Phase 3 - UI & Document Intelligence (Sprint 3)
- Clinical document intelligence functions (df_id_extractor, df_notes, fa_upload_doc)
- Health Web Portal (hc_ui/aihcweb)

## Phase 4 - Quality & Operations (Sprint 4)
- Playwright API test suite (hc_qa)
- DevOps pipelines (hc_ai_ops)
- Health demo app & script (hc_demo)
