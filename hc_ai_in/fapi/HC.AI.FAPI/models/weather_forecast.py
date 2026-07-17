from datetime import date

from pydantic import BaseModel, computed_field


class WeatherForecast(BaseModel):
    date: date
    temperature_c: int
    summary: str | None = None

    @computed_field
    @property
    def temperature_f(self) -> int:
        return 32 + int(self.temperature_c / 0.5556)
