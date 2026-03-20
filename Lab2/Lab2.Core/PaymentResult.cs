namespace Lab2.Core;

public record PaymentResult(
    bool Success,
    string TransactionId,
    string? ErrorMessage
);