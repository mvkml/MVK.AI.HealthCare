using HC.AI.Admin.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace HC.AI.Admin.EF.DBContexts;

public class AdminDbContext : DbContext
{
    public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options) { }

    public DbSet<AdminAccount> Admins => Set<AdminAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminAccount>(e =>
        {
            e.HasKey(a => a.AdminId);
            e.Property(a => a.FullName).IsRequired().HasMaxLength(150);
            e.Property(a => a.Email).IsRequired().HasMaxLength(255);
            e.HasIndex(a => a.Email).IsUnique();
            e.Property(a => a.PasswordHash).IsRequired().HasMaxLength(255);
        });
    }
}
