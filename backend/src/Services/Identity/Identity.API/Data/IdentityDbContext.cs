using Identity.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Data;

public class IdentityDbContext : IdentityDbContext<AppUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public DbSet<Address> Addresses => Set<Address>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Address>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Label).HasMaxLength(50);
            entity.Property(a => a.FirstName).HasMaxLength(100);
            entity.Property(a => a.LastName).HasMaxLength(100);
            entity.Property(a => a.AddressLine).HasMaxLength(250);
            entity.Property(a => a.Country).HasMaxLength(100);
            entity.Property(a => a.State).HasMaxLength(100);
            entity.Property(a => a.ZipCode).HasMaxLength(20);
            entity.Property(a => a.EmailAddress).HasMaxLength(150);
            entity.HasIndex(a => a.UserId);
        });
    }
}
