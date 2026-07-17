import random
from datetime import date, timedelta

from fastapi import APIRouter

from models.weather_forecast import WeatherForecast

router = APIRouter(prefix="/weatherforecast", tags=["weatherforecast"])

SUMMARIES = [
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm",
    "Balmy", "Hot", "Sweltering", "Scorching",
]


@router.get("", response_model=list[WeatherForecast])
def get_weather_forecast() -> list[WeatherForecast]:
    return [
        WeatherForecast(
            date=date.today() + timedelta(days=i),
            temperature_c=random.randint(-20, 55),
            summary=random.choice(SUMMARIES),
        )
        for i in range(1, 6)
    ]
