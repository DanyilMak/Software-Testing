namespace Lab2.Core;

public record Order(
    int Id,
    int CustomerId,
    string CustomerEmail,
    List<OrderItem> Items,
    decimal TotalAmount,
    OrderStatus Status
);