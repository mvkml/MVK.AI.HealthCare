# What is `winutils.exe` and why Spark needs it on Windows

## 1. The core issue

Apache Spark's file-handling code is built on top of **Apache Hadoop's** file system
libraries (`org.apache.hadoop.fs.*`), even when you are **not** using HDFS and only
reading/writing files on your local disk. This is true for a plain `local[*]` Spark
session as well as a full cluster.

Those Hadoop libraries were originally written assuming a POSIX-style OS (Linux/Mac),
where checking or setting file permissions, creating temp/shuffle directories, and
similar low-level file operations are done through standard Unix system calls.

Windows does not have those same system calls. So on Windows, Hadoop's code instead
looks for a small native helper program called `winutils.exe` to perform the
equivalent operations (permission checks, POSIX-style file mode emulation, etc.).

## 2. What breaks without it

If `winutils.exe` is missing, you will hit errors such as:

```
java.io.IOException: Could not locate executable null\bin\winutils.exe in the Hadoop binaries.
```

This shows up the moment Spark tries to:
- Write shuffle/temp files during a `.groupBy()`, `.join()`, or any wide transformation
- Save a DataFrame with `.write.csv(...)` / `.write.parquet(...)`
- Initialize the default Hive warehouse directory (created automatically by `SparkSession`)

Note: it happens **even in local mode with no HDFS involved** — this trips up almost
everyone setting up Spark on Windows for the first time.

## 3. What `hadoop.dll` is for

`hadoop.dll` is the native Windows library that `winutils.exe` (and Hadoop's native I/O
layer inside the JVM) links against, for things like file permission bits and native
I/O calls. It needs to sit next to `winutils.exe` in the same `bin` folder, and
ideally also be reachable from `PATH` (or copied into `C:\Windows\System32`) so the
JVM can load it at runtime.

## 4. Where it comes from / size

- Hadoop itself does not ship Windows binaries in its standard release — you have to
  build them yourself from source, or use a prebuilt community mirror.
- The de-facto standard, widely used and referenced across Spark documentation and
  tutorials, is **`cdarlint/winutils`** on GitHub — a repo of prebuilt `winutils.exe` +
  `hadoop.dll` pairs for every Hadoop version.
- For our setup we need the **Hadoop 3.3.4** build (matching the version bundled
  inside Spark 3.5.5's jars).
- Both files are tiny native executables:
  - `winutils.exe` ≈ 100–150 KB
  - `hadoop.dll` ≈ 100–150 KB
  - Combined, well under 1 MB total.

## 5. What we do with them

1. Create a folder to act as a fake "Hadoop install", e.g. `C:\hadoop\bin`
2. Place `winutils.exe` and `hadoop.dll` inside it
3. Set `HADOOP_HOME=C:\hadoop` (Spark/Hadoop code looks for `%HADOOP_HOME%\bin\winutils.exe`)
4. Add `C:\hadoop\bin` to `PATH` so `hadoop.dll` can be loaded by the JVM

No installation, no admin rights, no registry changes — it's just two files dropped
into a folder plus one environment variable.

## 6. Do we strictly need it?

Yes, for any real work — even the very first `df.show()` on a DataFrame you built
from a CSV will attempt to write to a local temp/warehouse directory and will fail
without `winutils.exe` present. It's a one-time, ~5 minute setup step.
