using Microsoft.EntityFrameworkCore;
using Order.Domain.Models;

namespace Order.Application.Data;

public interface IOrderDbContext
{
    DbSet<Domain.Models.Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
