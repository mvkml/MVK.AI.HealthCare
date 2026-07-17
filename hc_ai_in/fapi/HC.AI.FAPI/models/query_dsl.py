from typing import Any, Optional

from pydantic import BaseModel


class QueryFilter(BaseModel):
    field: str
    op: str = "eq"
    value: Any = None


class QueryOrderBy(BaseModel):
    field: str
    direction: str = "asc"


class QueryRequest(BaseModel):
    table: str
    select: Optional[list[str]] = None
    filters: Optional[list[QueryFilter]] = None
    order_by: Optional[QueryOrderBy] = None
    limit: Optional[int] = None


class QueryResult(BaseModel):
    success: bool
    table: str
    rows: list[dict[str, Any]] = []
    row_count: int = 0
    error: Optional[str] = None
