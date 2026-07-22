using HC.AI.Identity.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace HC.AI.Identity.EF.DBContexts
{
    public class AiHrDbContext : DbContext
    {
        public AiHrDbContext(DbContextOptions<AiHrDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<OcrDocument> OcrDocuments => Set<OcrDocument>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(e =>
            {
                e.HasKey(r => r.RoleId);
                e.Property(r => r.RoleName).IsRequired().HasMaxLength(100);
                e.HasIndex(r => r.RoleName).IsUnique();
                e.Property(r => r.OrderId).IsRequired();
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.UserId);
                e.Property(u => u.FullName).IsRequired().HasMaxLength(150);
                e.Property(u => u.Email).IsRequired().HasMaxLength(255);
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Company).IsRequired().HasMaxLength(150);
                e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);

                e.HasOne(u => u.Role)
                 .WithMany(r => r.Users)
                 .HasForeignKey(u => u.RoleId);
            });

            modelBuilder.Entity<OcrDocument>(e =>
            {
                e.HasKey(o => o.Id);
                e.Property(o => o.DocumentNumber).IsRequired().HasMaxLength(50);
                e.HasIndex(o => o.DocumentNumber).IsUnique();
                e.Property(o => o.GroupNumber).HasMaxLength(50);
                e.Property(o => o.FileName).IsRequired().HasMaxLength(255);
                e.Property(o => o.FileExtension).IsRequired().HasMaxLength(10);
                e.Property(o => o.DocumentType).IsRequired().HasMaxLength(50);
                e.Property(o => o.SourceLocation).HasMaxLength(500);
                e.Property(o => o.BlobUrl).HasMaxLength(1000);
                e.Property(o => o.DfInstanceId).HasMaxLength(100);
                e.Property(o => o.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Pending");
                e.Property(o => o.IsActive).HasDefaultValue(true);
                e.Property(o => o.CreatedDateTime).IsRequired();
            });

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Doctor", OrderId = 1 },
                new Role { RoleId = 2, RoleName = "Patient", OrderId = 2 }
            );
        }
    }
}
