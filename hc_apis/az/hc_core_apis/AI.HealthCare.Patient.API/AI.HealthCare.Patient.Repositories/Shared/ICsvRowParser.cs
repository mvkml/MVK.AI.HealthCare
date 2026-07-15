namespace AI.HealthCare.Patient.Repositories;

public interface ICsvRowParser<TModel>
{
    TModel ToModel(string[] row);
}
