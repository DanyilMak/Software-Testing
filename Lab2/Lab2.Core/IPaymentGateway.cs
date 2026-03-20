namespace Lab2.Core;

public interface IPaymentGateway
{
    PaymentResult ProcessPayment(decimal amount, string currency);
}