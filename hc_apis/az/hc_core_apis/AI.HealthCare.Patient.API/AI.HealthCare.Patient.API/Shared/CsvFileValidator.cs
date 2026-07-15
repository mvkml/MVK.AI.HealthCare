using Microsoft.AspNetCore.Http;

namespace AI.HealthCare.Patient.API.Shared;

public class CsvFileValidator : ICsvFileValidator
{
    private const long MaxFileSizeBytes = 100 * 1024 * 1024; // 100MB

    public (bool IsValid, string? ErrorMessage) Validate(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return (false, "File is empty or missing.");

        if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            return (false, "Only .csv files are supported.");

        if (file.Length > MaxFileSizeBytes)
            return (false, $"File exceeds the maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)}MB.");

        return (true, null);
    }
}
