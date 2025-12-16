using Microsoft.AspNetCore.Http;

namespace SomosRentWi.Application.Cars.DTOs;

public class UpdateCarRequest
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Plate { get; set; }
    public string? Color { get; set; }
    
    public decimal? CommercialValue { get; set; }
    public decimal? BasePricePerHour { get; set; }
    
    public DateTime? SoatExpirationDate { get; set; }
    public DateTime? TechExpirationDate { get; set; }
    
    public IFormFile? MainPhoto { get; set; }
}
