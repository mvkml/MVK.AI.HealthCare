from fastapi import FastAPI

from routers import weatherforecast

app = FastAPI(title="HC.AI.FAPI")

app.include_router(weatherforecast.router)


@app.get("/")
def read_root():
    return {"status": "ok"}
