namespace SomosRentWi.Domain.Enums;

public enum RentalStatus
{
    PendingDelivery,
    InProgress,
    FinishedCorrect, 
    FinishedWithIssue,
    Cancelled,
    Rejected
}