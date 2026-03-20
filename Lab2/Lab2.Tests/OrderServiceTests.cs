using Lab2.Core;
using NSubstitute;
using Shouldly;
using Xunit;

public class OrderServiceTests
{
    [Fact]
    public void PlaceOrder_WhenPaymentSucceeds_SavesOrderAndSendsConfirmation()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var payment = Substitute.For<IPaymentGateway>();
        var notifications = Substitute.For<INotificationService>();
        payment.ProcessPayment(Arg.Any<decimal>(), "USD")
            .Returns(new PaymentResult(true, "TX-123", null));
        var sut = new OrderService(repo, payment, notifications);
        var items = new List<OrderItem> { new("Book", 2, 10m) };

        // Act
        sut.PlaceOrder(1, "test@example.com", items, "USD");

        // Assert
        repo.Received(1).Save(Arg.Is<Order>(o => o.Status == OrderStatus.Confirmed));
        notifications.Received(1).SendOrderConfirmation("test@example.com", Arg.Any<int>());
    }

    [Fact]
    public void PlaceOrder_WhenPaymentFails_ThrowsException()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var payment = Substitute.For<IPaymentGateway>();
        var notifications = Substitute.For<INotificationService>();
        payment.ProcessPayment(Arg.Any<decimal>(), "USD")
            .Returns(new PaymentResult(false, null, "Insufficient funds"));
        var sut = new OrderService(repo, payment, notifications);
        var items = new List<OrderItem> { new("Book", 1, 20m) };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
            sut.PlaceOrder(1, "fail@example.com", items, "USD"));
    }

    [Fact]
    public void CancelOrder_WhenOrderIsShipped_ThrowsException()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var payment = Substitute.For<IPaymentGateway>();
        var notifications = Substitute.For<INotificationService>();
        var shippedOrder = new Order(1, 1, "test@example.com", new(), 100m, OrderStatus.Shipped);
        repo.GetById(1).Returns(shippedOrder);
        var sut = new OrderService(repo, payment, notifications);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => sut.CancelOrder(1));
    }

    [Fact]
    public void CancelOrder_WhenOrderIsPending_SavesCancelledOrderAndSendsNotification()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var payment = Substitute.For<IPaymentGateway>();
        var notifications = Substitute.For<INotificationService>();
        var pendingOrder = new Order(1, 1, "test@example.com", new(), 50m, OrderStatus.Pending);
        repo.GetById(1).Returns(pendingOrder);
        var sut = new OrderService(repo, payment, notifications);

        // Act
        sut.CancelOrder(1);

        // Assert
        repo.Received(1).Save(Arg.Is<Order>(o => o.Status == OrderStatus.Cancelled));
        notifications.Received(1).SendOrderCancellation("test@example.com", 1);
    }

    [Theory]
    [InlineData(100, true)]
    [InlineData(200, false)]
    public void PlaceOrder_WithDifferentPaymentResults_BehavesCorrectly(decimal amount, bool success)
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var payment = Substitute.For<IPaymentGateway>();
        var notifications = Substitute.For<INotificationService>();
        payment.ProcessPayment(amount, "USD")
            .Returns(new PaymentResult(success, success ? "TX-OK" : null, success ? null : "Error"));
        var sut = new OrderService(repo, payment, notifications);
        var items = new List<OrderItem> { new("Item", 1, amount) };

        // Act & Assert
        if (success)
        {
            sut.PlaceOrder(1, "ok@example.com", items, "USD");
            repo.Received(1).Save(Arg.Is<Order>(o => o.Status == OrderStatus.Confirmed));
        }
        else
        {
            Should.Throw<InvalidOperationException>(() =>
                sut.PlaceOrder(1, "fail@example.com", items, "USD"));
            notifications.DidNotReceive().SendOrderConfirmation(Arg.Any<string>(), Arg.Any<int>());
        }
    }

    [Fact]
    public void GetOrderHistory_ReturnsOrdersFromRepository()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var payment = Substitute.For<IPaymentGateway>();
        var notifications = Substitute.For<INotificationService>();
        var orders = new List<Order> { new(1, 1, "a@b.com", new(), 10m, OrderStatus.Confirmed) };
        repo.GetByCustomerId(1).Returns(orders);
        var sut = new OrderService(repo, payment, notifications);

        // Act
        var result = sut.GetOrderHistory(1);

        // Assert
        result.ShouldBe(orders);
    }

    [Fact]
    public void PlaceOrder_VerifiesOrderSavedBeforeNotification()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var payment = Substitute.For<IPaymentGateway>();
        var notifications = Substitute.For<INotificationService>();
        payment.ProcessPayment(Arg.Any<decimal>(), "USD")
            .Returns(new PaymentResult(true, "TX-123", null));
        var sut = new OrderService(repo, payment, notifications);
        var items = new List<OrderItem> { new("Book", 1, 10m) };

        // Act
        sut.PlaceOrder(1, "order@example.com", items, "USD");

        // Assert
        Received.InOrder(() =>
        {
            repo.Save(Arg.Any<Order>());
            notifications.SendOrderConfirmation("order@example.com", Arg.Any<int>());
        });
    }

    [Fact]
    public void CancelOrder_DoesNotSendConfirmation()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var payment = Substitute.For<IPaymentGateway>();
        var notifications = Substitute.For<INotificationService>();
        var order = new Order(1, 1, "cancel@example.com", new(), 20m, OrderStatus.Pending);
        repo.GetById(1).Returns(order);
        var sut = new OrderService(repo, payment, notifications);

        // Act
        sut.CancelOrder(1);

        // Assert
        notifications.DidNotReceive().SendOrderConfirmation(Arg.Any<string>(), Arg.Any<int>());
    }

    [Fact]
    public void PlaceOrder_CapturesCorrectTotalAmount()
    {
        // Arrange
        var repo = Substitute.For<IOrderRepository>();
        var payment = Substitute.For<IPaymentGateway>();
        var notifications = Substitute.For<INotificationService>();
        payment.ProcessPayment(Arg.Any<decimal>(), "USD")
            .Returns(new PaymentResult(true, "TX-123", null));
        var sut = new OrderService(repo, payment, notifications);
        var items = new List<OrderItem>
        {
            new("Pen", 2, 5m),
            new("Notebook", 1, 15m)
        };

        // Act
        sut.PlaceOrder(1, "capture@example.com", items, "USD");

        // Assert
        repo.Received().Save(Arg.Is<Order>(o => o.TotalAmount == 25m));
    }
}
