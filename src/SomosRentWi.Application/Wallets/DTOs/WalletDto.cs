namespace SomosRentWi.Application.Wallets.DTOs;

public class WalletDto
{
    public decimal Balance { get; set; }
    public List<TransactionDto> Transactions { get; set; } = new();
}
