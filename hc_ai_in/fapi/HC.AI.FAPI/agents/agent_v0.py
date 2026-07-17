from models.query_dsl import QueryRequest, QueryResult
from tools.healthcare_query_tool import HealthcareQueryTool


class AgentV0:
    def __init__(self, query_tool: HealthcareQueryTool):
        self._query_tool = query_tool

    async def handle_request(self, user_question: str) -> str:
        raise NotImplementedError

    async def _build_query(self, user_question: str) -> QueryRequest:
        raise NotImplementedError

    def _format_response(self, result: QueryResult) -> str:
        raise NotImplementedError
