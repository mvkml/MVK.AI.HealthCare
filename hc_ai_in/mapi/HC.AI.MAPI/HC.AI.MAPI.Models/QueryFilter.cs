namespace HC.AI.MAPI.Models;

public class QueryFilter
{
    public string Field { get; set; } = string.Empty;
    public string Op { get; set; } = "eq";
    public object? Value { get; set; }
}
