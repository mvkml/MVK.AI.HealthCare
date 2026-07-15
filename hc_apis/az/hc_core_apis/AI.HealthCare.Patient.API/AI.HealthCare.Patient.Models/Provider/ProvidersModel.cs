namespace AI.HealthCare.Patient.Models.Provider;

public class ProvidersModel : BaseModel
{
    public ProviderRequest ProviderRequest { get; set; } = new();
    public ProviderItem ProviderItem { get; set; } = new();
    public List<ProviderItem> ProviderItems { get; set; } = new();
    public ProviderResponse ProviderResponse { get; set; } = new();
}
