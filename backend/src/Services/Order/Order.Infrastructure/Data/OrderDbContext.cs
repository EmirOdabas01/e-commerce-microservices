using Microsoft.EntityFrameworkCore;
using Order.Application.Data;
using Order.Domain.Models;

namespace Order.Infrastructure.Data;

public class OrderDbContext : DbContext, IOrderDbContext
{
    public DbSet<Domain.Models.Order> Orders => Set<Domain.Models.Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Models.Order>(builder =>
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.UserName).IsRequired().HasMaxLength(150);
            builder.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");
            builder.Property(o => o.Status).HasConversion<string>();

            builder.OwnsOne(o => o.ShippingAddress, a =>
            {
                a.Property(p => p.FirstName).HasMaxLength(100);
                a.Property(p => p.LastName).HasMaxLength(100);
                a.Property(p => p.EmailAddress).HasMaxLength(256);
                a.Property(p => p.AddressLine).HasMaxLength(256);
                a.Property(p => p.Country).HasMaxLength(100);
                a.Property(p => p.State).HasMaxLength(100);
                a.Property(p => p.ZipCode).HasMaxLength(20);
            });

            builder.OwnsOne(o => o.PaymentInfo, p =>
            {
                p.Property(pp => pp.CardName).HasMaxLength(100);
                p.Property(pp => pp.CardNumber).HasMaxLength(30);
                p.Property(pp => pp.Expiration).HasMaxLength(10);
                p.Property(pp => pp.Cvv).HasMaxLength(5);
            });

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);

            builder.Ignore(o => o.DomainEvents);
        });

        modelBuilder.Entity<OrderItem>(builder =>
        {
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.ProductName).IsRequired().HasMaxLength(200);
            builder.Property(oi => oi.Price).HasColumnType("decimal(18,2)");
        });
    }
}
