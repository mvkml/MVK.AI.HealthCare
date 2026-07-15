using Microsoft.AspNetCore.Http;

namespace AI.HealthCare.Patient.API.Shared;

public interface ICsvFileValidator
{
    (bool IsValid, string? ErrorMessage) Validate(IFormFile? file);
}
