using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Domain.Entities;

public class Rental
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public required Client Client { get; set;}
    
    public int CompanyId { get; set; }
    public required Company Company { get; set;}
    
    public int CarId { get; set; }
    public required Car Car { get; set;}

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    
    public decimal PricePerHour { get; set; } // Snapshot at booking time
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; }

    public RentalStatus Status { get; set; } = RentalStatus.PendingDelivery;
    
    public string ContractPdfUrl { get; set; } = string.Empty;
}