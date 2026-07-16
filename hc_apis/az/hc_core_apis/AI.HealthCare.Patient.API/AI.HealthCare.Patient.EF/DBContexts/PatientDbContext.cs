using AI.HealthCare.Patient.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI.HealthCare.Patient.EF.DBContexts;

public class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options) { }

    public DbSet<Entities.Patient> Patients => Set<Entities.Patient>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Payer> Payers => Set<Payer>();
    public DbSet<PayerTransition> PayerTransitions => Set<PayerTransition>();
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<Condition> Conditions => Set<Condition>();
    public DbSet<Allergy> Allergies => Set<Allergy>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<Careplan> Careplans => Set<Careplan>();
    public DbSet<Procedure> Procedures => Set<Procedure>();
    public DbSet<Immunization> Immunizations => Set<Immunization>();
    public DbSet<Observation> Observations => Set<Observation>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Supply> Supplies => Set<Supply>();
    public DbSet<ImagingStudy> ImagingStudies => Set<ImagingStudy>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<ClaimTransaction> ClaimTransactions => Set<ClaimTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entities.Patient>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).ValueGeneratedNever();
            e.Property(p => p.Ssn).HasMaxLength(20);
            e.Property(p => p.Drivers).HasMaxLength(20);
            e.Property(p => p.Passport).HasMaxLength(20);
            e.Property(p => p.Prefix).HasMaxLength(100);
            e.Property(p => p.First).IsRequired().HasMaxLength(100);
            e.Property(p => p.Middle).HasMaxLength(100);
            e.Property(p => p.Last).IsRequired().HasMaxLength(100);
            e.Property(p => p.Suffix).HasMaxLength(100);
            e.Property(p => p.Maiden).HasMaxLength(100);
            e.Property(p => p.Marital).HasMaxLength(10);
            e.Property(p => p.Race).HasMaxLength(30);
            e.Property(p => p.Ethnicity).HasMaxLength(30);
            e.Property(p => p.Gender).HasMaxLength(30);
            e.Property(p => p.Birthplace).HasMaxLength(150);
            e.Property(p => p.Address).HasMaxLength(150);
            e.Property(p => p.City).HasMaxLength(150);
            e.Property(p => p.State).HasMaxLength(150);
            e.Property(p => p.County).HasMaxLength(150);
            e.Property(p => p.Fips).HasMaxLength(10);
            e.Property(p => p.Zip).HasMaxLength(10);
            e.Property(p => p.Lat).HasColumnType("decimal(10,7)");
            e.Property(p => p.Lon).HasColumnType("decimal(10,7)");
            e.Property(p => p.HealthcareExpenses).HasColumnType("decimal(12,2)");
            e.Property(p => p.HealthcareCoverage).HasColumnType("decimal(12,2)");
        });

        modelBuilder.Entity<Organization>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).ValueGeneratedNever();
            e.Property(o => o.Name).IsRequired().HasMaxLength(200);
            e.Property(o => o.Address).HasMaxLength(200);
            e.Property(o => o.City).HasMaxLength(150);
            e.Property(o => o.State).HasMaxLength(150);
            e.Property(o => o.Zip).HasMaxLength(10);
            e.Property(o => o.Phone).HasMaxLength(50);
            e.Property(o => o.Lat).HasColumnType("decimal(10,7)");
            e.Property(o => o.Lon).HasColumnType("decimal(10,7)");
            e.Property(o => o.Revenue).HasColumnType("decimal(14,2)");
        });

        modelBuilder.Entity<Provider>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).ValueGeneratedNever();
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.Gender).HasMaxLength(30);
            e.Property(p => p.Speciality).HasMaxLength(150);
            e.Property(p => p.Address).HasMaxLength(200);
            e.Property(p => p.City).HasMaxLength(150);
            e.Property(p => p.State).HasMaxLength(150);
            e.Property(p => p.Zip).HasMaxLength(10);
            e.Property(p => p.Lat).HasColumnType("decimal(10,7)");
            e.Property(p => p.Lon).HasColumnType("decimal(10,7)");

            e.HasOne(p => p.Organization)
             .WithMany()
             .HasForeignKey(p => p.OrganizationId);
        });

        modelBuilder.Entity<Payer>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).ValueGeneratedNever();
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.Ownership).HasMaxLength(50);
            e.Property(p => p.Address).HasMaxLength(200);
            e.Property(p => p.City).HasMaxLength(150);
            e.Property(p => p.StateHeadquartered).HasMaxLength(150);
            e.Property(p => p.Zip).HasMaxLength(10);
            e.Property(p => p.Phone).HasMaxLength(50);
            e.Property(p => p.AmountCovered).HasColumnType("decimal(14,2)");
            e.Property(p => p.AmountUncovered).HasColumnType("decimal(14,2)");
            e.Property(p => p.Revenue).HasColumnType("decimal(14,2)");
            e.Property(p => p.QolsAvg).HasColumnType("decimal(6,4)");
        });

        modelBuilder.Entity<PayerTransition>(e =>
        {
            e.HasKey(pt => pt.Id);
            e.Property(pt => pt.PlanOwnership).HasMaxLength(50);
            e.Property(pt => pt.OwnerName).HasMaxLength(200);

            e.HasOne(pt => pt.Patient)
             .WithMany()
             .HasForeignKey(pt => pt.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(pt => pt.Payer)
             .WithMany()
             .HasForeignKey(pt => pt.PayerId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(pt => pt.SecondaryPayer)
             .WithMany()
             .HasForeignKey(pt => pt.SecondaryPayerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Encounter>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.EncounterClass).HasMaxLength(30);
            e.Property(x => x.Code).HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.BaseEncounterCost).HasColumnType("decimal(12,2)");
            e.Property(x => x.TotalClaimCost).HasColumnType("decimal(12,2)");
            e.Property(x => x.PayerCoverage).HasColumnType("decimal(12,2)");
            e.Property(x => x.ReasonCode).HasMaxLength(50);
            e.Property(x => x.ReasonDescription).HasMaxLength(300);

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Organization)
             .WithMany()
             .HasForeignKey(x => x.OrganizationId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Provider)
             .WithMany()
             .HasForeignKey(x => x.ProviderId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Payer)
             .WithMany()
             .HasForeignKey(x => x.PayerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Condition>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.System).HasMaxLength(60);
            e.Property(x => x.Code).HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(300);

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Encounter)
             .WithMany()
             .HasForeignKey(x => x.EncounterId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Allergy>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(50);
            e.Property(x => x.System).HasMaxLength(60);
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.Type).HasMaxLength(30);
            e.Property(x => x.Category).HasMaxLength(30);
            e.Property(x => x.Reaction1).HasMaxLength(300);
            e.Property(x => x.Description1).HasMaxLength(300);
            e.Property(x => x.Severity1).HasMaxLength(30);
            e.Property(x => x.Reaction2).HasMaxLength(300);
            e.Property(x => x.Description2).HasMaxLength(300);
            e.Property(x => x.Severity2).HasMaxLength(30);

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Encounter)
             .WithMany()
             .HasForeignKey(x => x.EncounterId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Medication>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.BaseCost).HasColumnType("decimal(12,2)");
            e.Property(x => x.PayerCoverage).HasColumnType("decimal(12,2)");
            e.Property(x => x.TotalCost).HasColumnType("decimal(12,2)");
            e.Property(x => x.ReasonCode).HasMaxLength(50);
            e.Property(x => x.ReasonDescription).HasMaxLength(300);

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Payer)
             .WithMany()
             .HasForeignKey(x => x.PayerId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Encounter)
             .WithMany()
             .HasForeignKey(x => x.EncounterId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Careplan>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Code).HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.ReasonCode).HasMaxLength(50);
            e.Property(x => x.ReasonDescription).HasMaxLength(300);

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Encounter)
             .WithMany()
             .HasForeignKey(x => x.EncounterId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Procedure>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.System).HasMaxLength(60);
            e.Property(x => x.Code).HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.BaseCost).HasColumnType("decimal(12,2)");
            e.Property(x => x.ReasonCode).HasMaxLength(50);
            e.Property(x => x.ReasonDescription).HasMaxLength(300);

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Encounter)
             .WithMany()
             .HasForeignKey(x => x.EncounterId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Immunization>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.BaseCost).HasColumnType("decimal(12,2)");

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Encounter)
             .WithMany()
             .HasForeignKey(x => x.EncounterId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Observation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Category).HasMaxLength(30);
            e.Property(x => x.Code).HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.Value).HasMaxLength(50);
            e.Property(x => x.Units).HasMaxLength(20);
            e.Property(x => x.Type).HasMaxLength(20);

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Encounter)
             .WithMany()
             .HasForeignKey(x => x.EncounterId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Device>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.Udi).HasMaxLength(250);

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Encounter)
             .WithMany()
             .HasForeignKey(x => x.EncounterId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Supply>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(300);

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Encounter)
             .WithMany()
             .HasForeignKey(x => x.EncounterId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ImagingStudy>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SeriesUid).HasMaxLength(150);
            e.Property(x => x.BodysiteCode).HasMaxLength(50);
            e.Property(x => x.BodysiteDescription).HasMaxLength(300);
            e.Property(x => x.ModalityCode).HasMaxLength(20);
            e.Property(x => x.ModalityDescription).HasMaxLength(150);
            e.Property(x => x.InstanceUid).HasMaxLength(150);
            e.Property(x => x.SopCode).HasMaxLength(100);
            e.Property(x => x.SopDescription).HasMaxLength(300);
            e.Property(x => x.ProcedureCode).HasMaxLength(50);

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Encounter)
             .WithMany()
             .HasForeignKey(x => x.EncounterId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Claim>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Diagnosis1).HasMaxLength(20);
            e.Property(x => x.Diagnosis2).HasMaxLength(20);
            e.Property(x => x.Diagnosis3).HasMaxLength(20);
            e.Property(x => x.Diagnosis4).HasMaxLength(20);
            e.Property(x => x.Diagnosis5).HasMaxLength(20);
            e.Property(x => x.Diagnosis6).HasMaxLength(20);
            e.Property(x => x.Diagnosis7).HasMaxLength(20);
            e.Property(x => x.Diagnosis8).HasMaxLength(20);
            e.Property(x => x.Status1).HasMaxLength(20);
            e.Property(x => x.Status2).HasMaxLength(20);
            e.Property(x => x.StatusP).HasMaxLength(20);
            e.Property(x => x.Outstanding1).HasColumnType("decimal(12,2)");
            e.Property(x => x.Outstanding2).HasColumnType("decimal(12,2)");
            e.Property(x => x.OutstandingP).HasColumnType("decimal(12,2)");

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Provider)
             .WithMany()
             .HasForeignKey(x => x.ProviderId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.PrimaryPatientInsurance)
             .WithMany()
             .HasForeignKey(x => x.PrimaryPatientInsuranceId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.SecondaryPatientInsurance)
             .WithMany()
             .HasForeignKey(x => x.SecondaryPatientInsuranceId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ReferringProvider)
             .WithMany()
             .HasForeignKey(x => x.ReferringProviderId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.SupervisingProvider)
             .WithMany()
             .HasForeignKey(x => x.SupervisingProviderId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Appointment)
             .WithMany()
             .HasForeignKey(x => x.AppointmentId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ClaimTransaction>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Type).IsRequired().HasMaxLength(20);
            e.Property(x => x.Amount).HasColumnType("decimal(12,2)");
            e.Property(x => x.Method).HasMaxLength(20);
            e.Property(x => x.ProcedureCode).HasMaxLength(20);
            e.Property(x => x.Modifier1).HasMaxLength(10);
            e.Property(x => x.Modifier2).HasMaxLength(10);
            e.Property(x => x.Notes).HasMaxLength(250);
            e.Property(x => x.UnitAmount).HasColumnType("decimal(12,2)");
            e.Property(x => x.TransferOutId).HasMaxLength(50);
            e.Property(x => x.TransferType).HasMaxLength(50);
            e.Property(x => x.Payments).HasColumnType("decimal(12,2)");
            e.Property(x => x.Adjustments).HasColumnType("decimal(12,2)");
            e.Property(x => x.Transfers).HasColumnType("decimal(12,2)");
            e.Property(x => x.Outstanding).HasColumnType("decimal(12,2)");
            e.Property(x => x.LineNote).HasMaxLength(250);

            e.HasOne(x => x.Claim)
             .WithMany()
             .HasForeignKey(x => x.ClaimId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Patient)
             .WithMany()
             .HasForeignKey(x => x.PatientId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.PlaceOfService)
             .WithMany()
             .HasForeignKey(x => x.PlaceOfServiceId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Appointment)
             .WithMany()
             .HasForeignKey(x => x.AppointmentId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Provider)
             .WithMany()
             .HasForeignKey(x => x.ProviderId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.SupervisingProvider)
             .WithMany()
             .HasForeignKey(x => x.SupervisingProviderId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
