using SomosRentWi.Domain.Common;
using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Domain.Entities;

public class WalletTransaction : AuditableEntity
{
    public int Id { get; set; }
    public int CompanyWalletId { get; set; }
    public CompanyWallet? CompanyWallet { get; set; }
    
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public WalletTransactionType TransactionType { get; set; }
    
    // Explicit TransactionDate if needed separate from CreatedAt, 
    // but CreatedAt serves this purpose nicely. 
    // Adding explicit property for business domain clarity if requested.
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow; 
}