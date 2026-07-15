using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.Models.Patient;

namespace AI.HealthCare.Patient.API.Tests.Patient;

public class PatientValidationServiceTests
{
    private readonly PatientValidationService _service = new();

    private static PatientsModel ValidModel() => new()
    {
        PatientRequest = new PatientRequest
        {
            First = "John",
            Last = "Doe",
            BirthDate = new DateTime(1990, 1, 1),
        }
    };

    [Fact]
    public void Validate_ValidRequest_ReturnsNotInvalid()
    {
        var result = _service.Validate(ValidModel());

        Assert.False(result.IsNotValid);
        Assert.Equal("Validation passed.", result.Message);
    }

    [Fact]
    public void Validate_MissingFirst_ReturnsInvalid()
    {
        var model = ValidModel();
        model.PatientRequest.First = "";

        var result = _service.Validate(model);

        Assert.True(result.IsNotValid);
        Assert.Equal("First name is required.", result.Message);
    }

    [Fact]
    public void Validate_MissingLast_ReturnsInvalid()
    {
        var model = ValidModel();
        model.PatientRequest.Last = "   ";

        var result = _service.Validate(model);

        Assert.True(result.IsNotValid);
        Assert.Equal("Last name is required.", result.Message);
    }

    [Fact]
    public void Validate_FutureBirthDate_ReturnsInvalid()
    {
        var model = ValidModel();
        model.PatientRequest.BirthDate = DateTime.UtcNow.AddDays(1);

        var result = _service.Validate(model);

        Assert.True(result.IsNotValid);
        Assert.Equal("A valid BirthDate is required.", result.Message);
    }

    [Fact]
    public void Validate_DefaultBirthDate_ReturnsInvalid()
    {
        var model = ValidModel();
        model.PatientRequest.BirthDate = default;

        var result = _service.Validate(model);

        Assert.True(result.IsNotValid);
        Assert.Equal("A valid BirthDate is required.", result.Message);
    }

    [Fact]
    public void Validate_DeathDateBeforeBirthDate_ReturnsInvalid()
    {
        var model = ValidModel();
        model.PatientRequest.DeathDate = model.PatientRequest.BirthDate.AddDays(-1);

        var result = _service.Validate(model);

        Assert.True(result.IsNotValid);
        Assert.Equal("DeathDate cannot be earlier than BirthDate.", result.Message);
    }

    [Fact]
    public void Validate_DeathDateAfterBirthDate_ReturnsNotInvalid()
    {
        var model = ValidModel();
        model.PatientRequest.DeathDate = model.PatientRequest.BirthDate.AddYears(50);

        var result = _service.Validate(model);

        Assert.False(result.IsNotValid);
    }
}
