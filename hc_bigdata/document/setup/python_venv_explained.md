# What is a Python 3.12 Virtual Environment, and why we need one here

Related docs:
- `databricks_local_equivalent.md` — overall concept mapping
- `winutils_explained.md` — why `winutils.exe` is needed
- `setup_progress_status.md` — step 6 in the tracker refers to this

## 1. What a virtual environment is

A **virtual environment** ("venv") is a self-contained copy of Python, isolated from
your main/system Python install, with its own private folder for installed packages.

Without one, every `pip install X` you run goes into the **one global Python**
installed on your machine — shared by every project you ever work on. That causes
two classic problems:

| Problem | Example |
|---|---|
| Version conflicts | Project A needs `pandas==1.5`, Project B needs `pandas==2.2`. Installed globally, only one can win. |
| Unclear reproducibility | Six months from now, nobody remembers which packages were installed for which project, or why. |

A venv fixes this by giving **each project its own private Python + package folder**.
Installing something inside `hr_bigdata`'s venv has zero effect on any other project
on your machine (including other parts of `ai_hr`, or unrelated work).

## 2. Why specifically Python 3.12, and why a *new* one

Your machine already has two Python installs:

| Python version | Location | Usable with PySpark 3.5.5? |
|---|---|---|
| 3.14.0 | `C:\Python314\python.exe` (your default `python`) | ❌ Too new — PySpark doesn't ship wheels for it yet |
| 3.12 | `C:\Users\mvidh\AppData\Local\Programs\Python\Python312\python.exe` | ✅ Supported |

We can't just use the global 3.12 install directly either, because:
- It's still the **system-wide** 3.12, shared with anything else on your machine that
  happens to use that same interpreter
- Anyone else picking up this project later (or you, on a different machine) wouldn't
  know exactly which packages/versions were expected

So the plan is: create a **venv rooted in Python 3.12, living inside `hr_bigdata/`**,
used only for this big-data work. Nothing global gets touched.

## 3. How this helps our HR big-data process specifically

- **Isolation**: `pyspark`, `jupyterlab`, and (later) `delta-spark` versions are pinned
  exactly for this project, independent of whatever Python tooling the rest of
  `ai_hr` (e.g. any FastAPI service) ends up using.
- **Reproducibility**: once installed, we can freeze the exact package list to a
  `requirements.txt` inside `hr_bigdata/`, so the same environment can be recreated
  on another machine (or after a reinstall) with one command.
- **Safety**: if something goes wrong or a package upgrade breaks things, we just
  delete the venv folder and recreate it — the system Python installs are never
  touched or at risk.
- **Matches how Databricks itself works**: every Databricks cluster/notebook also
  runs inside its own isolated Python environment (the "Databricks Runtime" image) —
  this venv is the local, single-machine equivalent of that isolation.

## 4. What the actual steps will look like (step 6 in the tracker)

```powershell
# 1. Create the venv using the Python 3.12 interpreter specifically (not the default `python`, which is 3.14)
py -3.12 -m venv C:\git\v\ai_hr\hr_bigdata\.venv

# 2. Activate it (only affects the current terminal session)
C:\git\v\ai_hr\hr_bigdata\.venv\Scripts\Activate.ps1

# 3. Confirm it's using 3.12 now, isolated from the system
python --version
```

Once activated, `pip install pyspark==3.5.5 jupyterlab` (step 7 in the tracker) installs
those packages **only inside `.venv`**, not system-wide.

The `.venv` folder itself will be added to `.gitignore` — it's a local, regeneratable
artifact and should never be committed to the repo.

## 5. Nothing has been created yet

This document is explanation only. The venv itself (step 6) has not been created —
say the word and we'll run the commands above.
