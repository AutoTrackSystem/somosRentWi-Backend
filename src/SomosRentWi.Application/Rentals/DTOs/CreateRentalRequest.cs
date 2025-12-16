namespace SomosRentWi.Application.Rentals.DTOs;

public class CreateRentalRequest
{
    public int CarId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EstimatedEndDate { get; set; }
}
