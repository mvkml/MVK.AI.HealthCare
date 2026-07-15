using AI.HealthCare.Patient.Models.Patient;
using AI.HealthCare.Patient.Repositories;

namespace AI.HealthCare.Patient.BL;

public interface IPatientBLMapper : ICsvRowParser<PatientItem>
{
    PatientItem ToItem(PatientRequest request);
    PatientResponse ToResponse(PatientItem item, bool includePii);
}
