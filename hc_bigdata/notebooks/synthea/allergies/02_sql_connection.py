"""
Step 2 - Connect to SQL Server (mvkhc) via Spark JDBC

Second step of the Synthea CSV -> SQL Server pipeline.
Reads connection settings from hc_bigdata/config/sql_connection_config.json
and opens a Spark JDBC connection to the local SQLEXPRESS instance,
database mvkhc, using Windows Integrated Security.
This only tests the connection - no data written yet.

Run from an activated hc_bigdata venv:
    python 02_sql_connection.py
"""

import json
import os

# Windows Integrated Security over JDBC needs this native companion DLL
# (mssql-jdbc_auth-<version>-x64.dll) on java.library.path - pure Java/JDBC
# can't do local SSPI/Windows auth without it, same role winutils.exe plays for Hadoop.
# In local/client mode the driver JVM is already running by the time
# SparkSession.builder.config("spark.driver.extraJavaOptions", ...) executes,
# so it must be set via PYSPARK_SUBMIT_ARGS before pyspark launches that JVM.
native_lib_path = os.path.abspath("../../../native").replace("\\", "/")
os.environ["PYSPARK_SUBMIT_ARGS"] = (
    f'--driver-java-options "-Djava.library.path={native_lib_path}" pyspark-shell'
)
print("java.library.path ->", native_lib_path)

from pyspark.sql import SparkSession

with open("../../../config/sql_connection_config.json") as f:
    config = json.load(f)["sql_server"]

print("Loaded config:", config)

spark = (
    SparkSession.builder.appName("synthea_sql_connection")
    .master("local[*]")
    .config("spark.jars.packages", config["maven_package"])
    .getOrCreate()
)

print("Spark session created:", spark)

df = spark.read.jdbc(
    url=config["jdbc_url"],
    table="(SELECT 1 AS test_col) AS connection_check",
    properties={"driver": config["driver_class"]},
)
df.show()

spark.stop()
