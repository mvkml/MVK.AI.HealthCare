namespace HC.AI.MAPI.Models;

public class QueryResult
{
    public bool Success { get; set; }
    public string Table { get; set; } = string.Empty;
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
    public int RowCount { get; set; }
    public string? Error { get; set; }
}
