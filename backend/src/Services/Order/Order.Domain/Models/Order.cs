using Order.Domain.Abstractions;
using Order.Domain.Enums;
using Order.Domain.Events;
using Order.Domain.ValueObjects;

namespace Order.Domain.Models;

public class Order : Aggregate<Guid>
{
    public string UserName { get; set; } = default!;
    public decimal TotalPrice { get; set; }
    public Address ShippingAddress { get; set; } = default!;
    public Payment PaymentInfo { get; set; } = default!;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public void AddItem(Guid productId, string productName, decimal price, int quantity)
    {
        var item = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = Id,
            ProductId = productId,
            ProductName = productName,
            Price = price,
            Quantity = quantity
        };

        _items.Add(item);
        TotalPrice += price * quantity;
    }

    public static Order Create(
        string userName,
        Address shippingAddress,
        Payment paymentInfo)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            ShippingAddress = shippingAddress,
            PaymentInfo = paymentInfo,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id));
        return order;
    }
}
