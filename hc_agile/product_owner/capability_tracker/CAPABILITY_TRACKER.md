# 📦 mvkhc — Capability Tracker
# Owner: Product Owner Agent
# Date: 2026-07-15
# Version: 1.0

---

## Purpose
This file tracks all mvkhc capabilities, achievements, and integrations.
Updated every sprint by the Product Owner.

---

## 1. Business Capabilities

| # | Capability | Status | Sprint Added |
|---|-----------|--------|--------------|
| 1 | Synthea Patient Data Exploration | 🔄 In Progress | Sprint 01 |
| 2 | Patient Management (AI.HealthCare.Patient.API) | 🔄 Planned | Sprint 02 |
| 3 | Core Healthcare Services (AI.HC.Api) | 🔄 Planned | Sprint 02 |
| 4 | Clinical Document Intelligence (upload/extract/notes) | 🔄 Planned | Sprint 03 |
| 5 | Health Web Portal | 🔄 Planned | Sprint 03 |

---

## 2. Technology Stack

| # | Technology | Purpose | Status |
|---|-----------|---------|--------|
| 1 | PySpark (local, Spark 3.5.5) | Synthea data exploration | ✅ Active |
| 2 | Synthea synthetic patient dataset | Source data (19 CSV tables) | ✅ Active |
| 3 | .NET (AI.HC.Api, AI.HealthCare.Patient.API) | Backend APIs | 🔄 Planned |
| 4 | Azure Functions (df_id_extractor, df_notes, fa_upload_doc) | Document intelligence | 🔄 Planned |
| 5 | Angular (hc_ui/aihcweb) | Frontend UI | 🔄 Planned |
| 6 | Playwright | API test automation | 🔄 Planned |
| 7 | Claude AI | AI Agents | ✅ Active |

---

## 3. Business Achievements

| # | Achievement | Date | Sprint |
|---|------------|------|--------|
| 1 | PySpark environment verified locally | 2026-07-15 | Sprint 01 |
| 2 | Allergies Synthea table profiled (schema + top 10) | 2026-07-15 | Sprint 01 |
| 3 | Agile backlog, roadmap, and sprint 1 plan established | 2026-07-15 | Sprint 01 |

---

## 4. Team / Agents

| # | Agent | Role | Status |
|---|-------|------|--------|
| 1 | 🏗️ Architect Agent | System Design | ✅ Active |
| 2 | 📦 Product Owner Agent | Product Vision | ✅ Active |
| 3 | 🏃 Scrum Master Agent | Delivery | ✅ Active |
| 4 | ⚡ Dev Angular Agent | Frontend | 👀 Observing |
| 5 | 🗄️ Dev SQL Agent | Database | 👀 Observing |
| 6 | 🧬 Dev BigData Agent | Synthea/PySpark | ✅ Active |
| 7 | 🔧 Dev DevOps Agent | Infrastructure | 👀 Observing |
| 8 | 🧪 Dev QA Agent | Test Automation | 👀 Observing |
| 9 | 🖥️ Dev .NET Agent | APIs | 👀 Observing |
| 10 | 🚀 Dev FastAPI Agent | (unused so far — no FastAPI service in this project) | 👀 Observing |

---

## 5. Update Log

| Date | Updated By | What Changed |
|------|-----------|--------------|
| 2026-07-15 | Product Owner | Initial capability tracker created, reflecting actual verified state (only hc_bigdata has real implemented work) |
