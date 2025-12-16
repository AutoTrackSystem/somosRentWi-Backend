using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Application.Cars.DTOs;

public class CarResponse
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    
    public decimal CommercialValue { get; set; }
    public decimal BasePricePerHour { get; set; }
    public decimal PricePerHour { get; set; }
    
    public DateTime SoatExpirationDate { get; set; }
    public DateTime TechExpirationDate { get; set; }
    
    public string MainPhotoUrl { get; set; } = string.Empty;
    public CarStatus Status { get; set; }
}
