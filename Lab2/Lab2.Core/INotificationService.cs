namespace Lab2.Core;

public interface INotificationService
{
    void SendOrderConfirmation(string email, int orderId);
    void SendOrderCancellation(string email, int orderId);
}