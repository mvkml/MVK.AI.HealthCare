namespace HC.AI.MAPI.Models;

public class QueryRequest
{
    public string Table { get; set; } = string.Empty;
    public List<string>? Select { get; set; }
    public List<QueryFilter>? Filters { get; set; }
    public QueryOrderBy? OrderBy { get; set; }
    public int? Limit { get; set; }
}
