using AI.HealthCare.Patient.Models.PayerTransition;
using EfPayerTransition = AI.HealthCare.Patient.EF.Entities.PayerTransition;

namespace AI.HealthCare.Patient.Repositories;

public class PayerTransitionMapper : IPayerTransitionMapper
{
    public PayerTransitionItem ToModel(EfPayerTransition entity) => new()
    {
        Id = entity.Id,
        PatientId = entity.PatientId,
        MemberId = entity.MemberId,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        PayerId = entity.PayerId,
        SecondaryPayerId = entity.SecondaryPayerId,
        PlanOwnership = entity.PlanOwnership,
        OwnerName = entity.OwnerName
    };

    public EfPayerTransition ToEntity(PayerTransitionItem item) => new()
    {
        Id = item.Id,
        PatientId = item.PatientId,
        MemberId = item.MemberId,
        StartDate = item.StartDate,
        EndDate = item.EndDate,
        PayerId = item.PayerId,
        SecondaryPayerId = item.SecondaryPayerId,
        PlanOwnership = item.PlanOwnership,
        OwnerName = item.OwnerName
    };
}
