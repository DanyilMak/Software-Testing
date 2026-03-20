namespace Lab2.Core;

public class OrderService
{
    private readonly IOrderRepository _repo;
    private readonly IPaymentGateway _payment;
    private readonly INotificationService _notifications;

    public OrderService(IOrderRepository repo, IPaymentGateway payment, INotificationService notifications)
    {
        _repo = repo;
        _payment = payment;
        _notifications = notifications;
    }

    public void PlaceOrder(int customerId, string email, List<OrderItem> items, string currency)
    {
        var total = items.Sum(i => i.Price * i.Quantity);
        var result = _payment.ProcessPayment(total, currency);

        if (!result.Success)
            throw new InvalidOperationException("Payment failed");

        var order = new Order(
            Id: new Random().Next(1, 1000),
            CustomerId: customerId,
            CustomerEmail: email,
            Items: items,
            TotalAmount: total,
            Status: OrderStatus.Confirmed
        );

        _repo.Save(order);
        _notifications.SendOrderConfirmation(email, order.Id);
    }

    public void CancelOrder(int orderId)
    {
        var order = _repo.GetById(orderId);
        if (order.Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Cannot cancel shipped order");

        var cancelled = order with { Status = OrderStatus.Cancelled };
        _repo.Save(cancelled);
        _notifications.SendOrderCancellation(order.CustomerEmail, order.Id);
    }

    public IEnumerable<Order> GetOrderHistory(int customerId) =>
        _repo.GetByCustomerId(customerId);
}
