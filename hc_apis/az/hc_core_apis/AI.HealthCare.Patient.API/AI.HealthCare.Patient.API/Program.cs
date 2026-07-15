using AI.HealthCare.Patient.API.Shared;
using AI.HealthCare.Patient.BL;
using AI.HealthCare.Patient.EF.DBContexts;
using AI.HealthCare.Patient.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PatientDb")));

builder.Services.AddScoped<IPatientMapper, PatientMapper>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IOrganizationMapper, OrganizationMapper>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IProviderMapper, ProviderMapper>();
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
builder.Services.AddScoped<IPayerMapper, PayerMapper>();
builder.Services.AddScoped<IPayerRepository, PayerRepository>();
builder.Services.AddScoped<IPayerTransitionMapper, PayerTransitionMapper>();
builder.Services.AddScoped<IPayerTransitionRepository, PayerTransitionRepository>();
builder.Services.AddScoped<IEncounterMapper, EncounterMapper>();
builder.Services.AddScoped<IEncounterRepository, EncounterRepository>();
builder.Services.AddScoped<IConditionMapper, ConditionMapper>();
builder.Services.AddScoped<IConditionRepository, ConditionRepository>();
builder.Services.AddScoped<IAllergyMapper, AllergyMapper>();
builder.Services.AddScoped<IAllergyRepository, AllergyRepository>();
builder.Services.AddScoped<IAllergyCsvMapper, AllergyCsvMapper>();
builder.Services.AddScoped<IMedicationMapper, MedicationMapper>();
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<ICareplanMapper, CareplanMapper>();
builder.Services.AddScoped<ICareplanRepository, CareplanRepository>();
builder.Services.AddScoped<IProcedureMapper, ProcedureMapper>();
builder.Services.AddScoped<IProcedureRepository, ProcedureRepository>();
builder.Services.AddScoped<IImmunizationMapper, ImmunizationMapper>();
builder.Services.AddScoped<IImmunizationRepository, ImmunizationRepository>();
builder.Services.AddScoped<IObservationMapper, ObservationMapper>();
builder.Services.AddScoped<IObservationRepository, ObservationRepository>();
builder.Services.AddScoped<IDeviceMapper, DeviceMapper>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<ISupplyMapper, SupplyMapper>();
builder.Services.AddScoped<ISupplyRepository, SupplyRepository>();
builder.Services.AddScoped<IImagingStudyMapper, ImagingStudyMapper>();
builder.Services.AddScoped<IImagingStudyRepository, ImagingStudyRepository>();
builder.Services.AddScoped<IClaimMapper, ClaimMapper>();
builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddScoped<IClaimTransactionMapper, ClaimTransactionMapper>();
builder.Services.AddScoped<IClaimTransactionRepository, ClaimTransactionRepository>();

builder.Services.AddScoped<IPatientBL, PatientBL>();
builder.Services.AddScoped<IPatientBLMapper, PatientBLMapper>();
builder.Services.AddScoped<IPatientValidationService, PatientValidationService>();

builder.Services.AddScoped<IOrganizationBL, OrganizationBL>();
builder.Services.AddScoped<IOrganizationBLMapper, OrganizationBLMapper>();
builder.Services.AddScoped<IOrganizationValidationService, OrganizationValidationService>();

builder.Services.AddScoped<IProviderBL, ProviderBL>();
builder.Services.AddScoped<IProviderBLMapper, ProviderBLMapper>();
builder.Services.AddScoped<IProviderValidationService, ProviderValidationService>();

builder.Services.AddScoped<IPayerBL, PayerBL>();
builder.Services.AddScoped<IPayerBLMapper, PayerBLMapper>();
builder.Services.AddScoped<IPayerValidationService, PayerValidationService>();

builder.Services.AddScoped<IPayerTransitionBL, PayerTransitionBL>();
builder.Services.AddScoped<IPayerTransitionBLMapper, PayerTransitionBLMapper>();
builder.Services.AddScoped<IPayerTransitionValidationService, PayerTransitionValidationService>();

builder.Services.AddScoped<IEncounterBL, EncounterBL>();
builder.Services.AddScoped<IEncounterBLMapper, EncounterBLMapper>();
builder.Services.AddScoped<IEncounterValidationService, EncounterValidationService>();

builder.Services.AddScoped<IConditionBL, ConditionBL>();
builder.Services.AddScoped<IConditionBLMapper, ConditionBLMapper>();
builder.Services.AddScoped<IConditionValidationService, ConditionValidationService>();

builder.Services.AddScoped<IAllergyBL, AllergyBL>();
builder.Services.AddScoped<IAllergyValidationService, AllergyValidationService>();
builder.Services.AddScoped<ICsvFileValidator, CsvFileValidator>();

builder.Services.AddScoped<IMedicationBL, MedicationBL>();
builder.Services.AddScoped<IMedicationBLMapper, MedicationBLMapper>();
builder.Services.AddScoped<IMedicationValidationService, MedicationValidationService>();

builder.Services.AddScoped<ICareplanBL, CareplanBL>();
builder.Services.AddScoped<ICareplanBLMapper, CareplanBLMapper>();
builder.Services.AddScoped<ICareplanValidationService, CareplanValidationService>();

builder.Services.AddScoped<IProcedureBL, ProcedureBL>();
builder.Services.AddScoped<IProcedureBLMapper, ProcedureBLMapper>();
builder.Services.AddScoped<IProcedureValidationService, ProcedureValidationService>();

builder.Services.AddScoped<IImmunizationBL, ImmunizationBL>();
builder.Services.AddScoped<IImmunizationBLMapper, ImmunizationBLMapper>();
builder.Services.AddScoped<IImmunizationValidationService, ImmunizationValidationService>();

builder.Services.AddScoped<IObservationBL, ObservationBL>();
builder.Services.AddScoped<IObservationValidationService, ObservationValidationService>();

builder.Services.AddScoped<IDeviceBL, DeviceBL>();
builder.Services.AddScoped<IDeviceBLMapper, DeviceBLMapper>();
builder.Services.AddScoped<IDeviceValidationService, DeviceValidationService>();

builder.Services.AddScoped<ISupplyBL, SupplyBL>();
builder.Services.AddScoped<ISupplyBLMapper, SupplyBLMapper>();
builder.Services.AddScoped<ISupplyValidationService, SupplyValidationService>();

builder.Services.AddScoped<IImagingStudyBL, ImagingStudyBL>();
builder.Services.AddScoped<IImagingStudyBLMapper, ImagingStudyBLMapper>();
builder.Services.AddScoped<IImagingStudyValidationService, ImagingStudyValidationService>();

builder.Services.AddScoped<IClaimBL, ClaimBL>();
builder.Services.AddScoped<IClaimBLMapper, ClaimBLMapper>();
builder.Services.AddScoped<IClaimValidationService, ClaimValidationService>();

builder.Services.AddScoped<IClaimTransactionBL, ClaimTransactionBL>();
builder.Services.AddScoped<IClaimTransactionValidationService, ClaimTransactionValidationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
