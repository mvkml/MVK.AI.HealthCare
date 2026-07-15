# Databricks → Local On-Premise PySpark: Concept Mapping

## 1. Important clarification first

Databricks has no true "on-premise" version — it is a commercial SaaS platform that only
runs on top of a cloud provider (AWS, Azure, or GCP). There is no installer for a laptop
or an on-prem server.

What we are actually building is the **open-source stack Databricks itself is built on**
(Apache Spark + notebooks), self-hosted on your laptop. It gives you the same PySpark
notebook experience — write PySpark queries, run them against a Spark engine, see
results in a table — without the Databricks product itself.

## 2. Concept mapping table

| # | Databricks concept | What it does in Databricks | Local / on-prem equivalent | Needed for us? |
|---|---|---|---|---|
| 1 | **Workspace** | Web UI hosting your notebooks, files, jobs | A project folder on disk (this `hr_bigdata/` folder) | Yes |
| 2 | **Notebook** (`.dbc` / hybrid) | Cells of SQL/Python/Scala mixed with markdown, run against a cluster | **Jupyter Notebook** (`.ipynb`) with a PySpark kernel | Yes |
| 3 | **Cluster** | Managed group of VMs running Spark, spun up/down on demand | A **local SparkSession** (`local[*]`) — Spark runs inside your own machine's cores, no separate VMs | Yes |
| 4 | **Databricks Runtime (DBR)** | Pre-bundled OS + Java + Python + Spark image the cluster boots from | Manually installed **Java 11 + Python 3.12 + Apache Spark 3.5.5** on your machine | Yes |
| 5 | **DBFS** (Databricks File System) | Virtual filesystem layered over cloud storage (S3/ADLS) | Your **local disk paths** (e.g. `C:\git\v\ai_hr\hr_bigdata\data`) | Yes (simplified) |
| 6 | **Delta Lake table** | Databricks' default table format (ACID, versioned, on top of Parquet) | Open-source `delta-spark` pip package — works identically outside Databricks | Optional (add later if you want table versioning) |
| 7 | **Hive Metastore / Unity Catalog** | Tracks table names → file locations, permissions | Spark's bundled local **Derby metastore** (automatic, zero setup) or just read/write files directly by path | Optional — skip initially |
| 8 | **`%sql` magic cell** | Run SQL directly in a notebook cell against a table | `spark.sql("SELECT ...")` in a Python cell, or a Jupyter SQL-magic extension | Yes, via `spark.sql()` |
| 9 | **Jobs / Workflows** | Scheduled/triggered notebook or script runs | Manual run for now; later, Windows Task Scheduler or Apache Airflow if needed | Not needed yet |
| 10 | **`dbutils.secrets`** | Managed secrets store | `.env` file (gitignored) or Windows environment variables | Only if we connect to real data sources |
| 11 | **Cluster autoscaling / multiple nodes** | Distributes work across many machines | Not applicable — single laptop = single node | N/A for now |

## 3. What "a table" looks like in this setup

In Databricks you'd do:
```python
df = spark.read.table("my_catalog.my_schema.employees")
display(df)
```

Locally (no catalog needed to start), the equivalent is:
```python
df = spark.read.csv("data/employees.csv", header=True, inferSchema=True)
df.show()          # text-table output
# or, in Jupyter:
df.toPandas()       # renders as a nice HTML table in the notebook
```

Once we register it as a temp view, you can query it with SQL exactly like Databricks:
```python
df.createOrReplaceTempView("employees")
spark.sql("SELECT department, COUNT(*) FROM employees GROUP BY department").show()
```

## 4. Requirements checklist — current machine status

| Component | Purpose | Status on this machine |
|---|---|---|
| Java (JDK 11) | JVM that Spark runs on | ✅ Installed (`jdk-11.0.29.7-hotspot`) |
| Python 3.12 | Runs PySpark driver code | ✅ Installed |
| Apache Spark 3.5.5 (Hadoop 3) | The engine itself | ✅ Already downloaded at `C:\spark\spark-3.5.5-bin-hadoop3` |
| `SPARK_HOME` / `JAVA_HOME` env vars | Point Spark's scripts at the right folders | ✅ Fixed this session |
| `winutils.exe` + `hadoop.dll` (Hadoop 3.3.4) | Windows shim Spark needs for local file I/O | ⬜ Not yet installed |
| PySpark (pip package) | Python API to Spark, used inside notebooks | ⬜ Not yet installed |
| JupyterLab | Notebook UI — this is our "Databricks notebook" | ⬜ Not yet installed |
| Delta Lake (`delta-spark`) | Optional table format | ⬜ Not installed (optional) |

## 5. Proposed step-by-step plan

| Step | Action | Status |
|---|---|---|
| 1 | Fix `SPARK_HOME` and `JAVA_HOME` user env vars | Done |
| 2 | Install `winutils.exe` + `hadoop.dll` matching Hadoop 3.3.4, set `HADOOP_HOME` | Pending your go-ahead |
| 3 | Add Spark `bin` and Java `bin` to `PATH` | Pending |
| 4 | Create a Python 3.12 virtual environment inside `hr_bigdata/` | Pending |
| 5 | `pip install pyspark==3.5.5 jupyterlab` inside that venv | Pending |
| 6 | Launch JupyterLab, create first notebook, run a sample PySpark query against a small CSV to prove the whole chain works | Pending |

We will not touch installation steps 2–6 until you review this table and confirm the plan.
