using SomosRentWi.Application.Rentals.DTOs;

namespace SomosRentWi.Application.Rentals.Interfaces;

public interface IRentalService
{
    Task<RentalResponse> CreateRentalAsync(CreateRentalRequest request, int userId);
    Task<RentalResponse?> GetRentalByIdAsync(int id);
    Task<List<RentalResponse>> GetRentalsByClientIdAsync(int userId);
    Task<List<RentalResponse>> GetRentalsByCompanyIdAsync(int userId);
    Task<RentalResponse> DeliverRentalAsync(int rentalId, int userId);
    Task<RentalResponse> CompleteRentalAsync(int rentalId, int userId);
    Task<RentalResponse> CancelRentalAsync(int rentalId, string reason);
}
