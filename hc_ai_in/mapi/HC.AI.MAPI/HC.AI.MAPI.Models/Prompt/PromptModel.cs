namespace HC.AI.MAPI.Models;

public class PromptModel : BaseModel
{
    public PromptRequest PromptRequest { get; set; } = new PromptRequest();
    public PromptResponse PromptResponse { get; set; } = new PromptResponse();
    public PromptItem PromptItem { get; set; } = new PromptItem();
}
