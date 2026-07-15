# PySpark On-Premise Setup — Progress Status

This file tracks what has been completed vs. what remains, so we stay aligned before
each next step. See also:
- `databricks_local_equivalent.md` — concept mapping (Databricks vs local)
- `winutils_explained.md` — why `winutils.exe` is needed

## Status table

| # | Step | Detail | Status |
|---|---|---|---|
| 1 | `SPARK_HOME` env var | Set to `C:\spark\spark-3.5.5-bin-hadoop3` | ✅ Done |
| 2 | `JAVA_HOME` env var | Set to Java 11 (`jdk-11.0.29.7-hotspot`) | ✅ Done |
| 3 | `winutils.exe` + `hadoop.dll` | Downloaded (Hadoop 3.3.5 build) to `C:\hadoop\bin` | ✅ Done |
| 4 | `HADOOP_HOME` env var | Set to `C:\hadoop` | ✅ Done |
| 5 | `PATH` update | Added `C:\hadoop\bin`, `%SPARK_HOME%\bin`, `%JAVA_HOME%\bin` | ✅ Done |
| 6 | Python 3.12 virtual environment | Created at `hr_bigdata\.venv` (Python 3.12.3) | ✅ Done |
| 7 | Install `pyspark==3.5.5` + `jupyterlab` | Installed into `.venv` (jupyterlab 4.6.1) | ✅ Done |
| 8 | Sample PySpark notebook | `notebooks/01_first_pyspark_query.ipynb` created and executed successfully — read `data/employees.csv`, ran a `GROUP BY` SQL query, verified correct output | ✅ Done |

## Notes

- Steps 1–5 are environment variable changes at the **Windows user level**. They only
  take effect in **new** terminal/PowerShell sessions opened after the change — any
  terminal already open before this needs to be restarted to see them.
- `hr_bigdata/.venv` is gitignored — it's a regeneratable local artifact, never committed.
- All 8 setup steps are complete. The chain (Java 11 → Spark 3.5.5 → winutils →
  PySpark → JupyterLab) is verified working end-to-end on this machine.

## How to open and use it yourself

```powershell
# Activate the venv in a new terminal
C:\git\v\ai_hr\hr_bigdata\.venv\Scripts\Activate.ps1

# Launch JupyterLab
jupyter lab
```

Then open `notebooks/01_first_pyspark_query.ipynb` from the JupyterLab file browser —
this is the local equivalent of opening a notebook in the Databricks workspace UI.
