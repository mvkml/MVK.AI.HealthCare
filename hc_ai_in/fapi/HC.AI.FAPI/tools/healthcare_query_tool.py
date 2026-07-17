from models.query_dsl import QueryRequest, QueryResult
from utils.http_client import PatientApiClient


class HealthcareQueryTool:
    def __init__(self, api_client: PatientApiClient):
        self._api_client = api_client

    async def execute_query(self, request: QueryRequest) -> QueryResult:
        raise NotImplementedError
