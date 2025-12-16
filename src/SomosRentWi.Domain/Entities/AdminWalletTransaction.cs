using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Domain.Entities;

public class AdminWalletTransaction
{
    public int Id { get; set; }

    public int AdminWalletId { get; set; }
    public AdminWallet? AdminWallet { get; set; }

    public decimal Amount { get; set; }

    public WalletTransactionType TransactionType { get; set; }

    public string Description { get; set; } = string.Empty;
}