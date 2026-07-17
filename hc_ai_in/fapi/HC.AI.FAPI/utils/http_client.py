class PatientApiClient:
    def __init__(self, base_url: str):
        self._base_url = base_url

    async def post_query(self, query_json: str):
        raise NotImplementedError
