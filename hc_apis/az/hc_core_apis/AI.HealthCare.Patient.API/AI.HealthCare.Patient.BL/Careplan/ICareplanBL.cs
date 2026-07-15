using AI.HealthCare.Patient.Models.Careplan;
using AI.HealthCare.Patient.Models.Shared;

namespace AI.HealthCare.Patient.BL;

public interface ICareplanBL
{
    Task<CareplansModel> Create(CareplansModel careplansModel);
    Task<CareplansModel> GetById(CareplansModel careplansModel);
    Task<CareplansModel> GetAll(CareplansModel careplansModel);
    Task<CareplansModel> GetByPatientId(Guid patientId);
    Task<CareplansModel> Update(CareplansModel careplansModel);
    Task<CareplansModel> Delete(CareplansModel careplansModel);
    Task<ImportResult> Import(Stream csvStream);
}
