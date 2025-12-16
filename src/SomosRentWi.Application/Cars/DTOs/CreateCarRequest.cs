using Microsoft.AspNetCore.Http;
using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Application.Cars.DTOs;

public class CreateCarRequest
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    
    public decimal CommercialValue { get; set; }
    public decimal BasePricePerHour { get; set; }
    
    public DateTime SoatExpirationDate { get; set; }
    public DateTime TechExpirationDate { get; set; }
    
    public IFormFile? MainPhoto { get; set; }
}
