using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Application.Rentals.DTOs;

public class RentalResponse
{
    public int Id { get; set; }
    
    // Client Info
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    
    // Company Info
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    
    // Car Info
    public int CarId { get; set; }
    public string CarBrand { get; set; } = string.Empty;
    public string CarModel { get; set; } = string.Empty;
    public string CarPlate { get; set; } = string.Empty;
    public string CarPhotoUrl { get; set; } = string.Empty;
    
    // Rental Details
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; }
    public RentalStatus Status { get; set; }
    public string ContractPdfUrl { get; set; } = string.Empty;
}
