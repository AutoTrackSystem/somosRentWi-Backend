namespace SomosRentWi.Domain.Entities;

public class AdminWallet
{
    public int Id { get; set; }

    public decimal Balance { get; set; }

    public ICollection<AdminWalletTransaction> Transactions { get; set; }
        = new List<AdminWalletTransaction>();
}