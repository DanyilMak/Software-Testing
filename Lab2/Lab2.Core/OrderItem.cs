namespace Lab2.Core;

public record OrderItem(
    string ProductName,
    int Quantity,
    decimal Price
);