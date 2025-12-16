using SomosRentWi.Application.Rentals.DTOs;

namespace SomosRentWi.Application.Rentals.Interfaces;

public interface IRentalService
{
    Task<RentalResponse> CreateRentalAsync(CreateRentalRequest request, int clientId);
    Task<RentalResponse?> GetRentalByIdAsync(int id);
    Task<List<RentalResponse>> GetRentalsByClientIdAsync(int clientId);
    Task<List<RentalResponse>> GetRentalsByCompanyIdAsync(int companyId);
    Task<RentalResponse> DeliverRentalAsync(int rentalId, int companyId);
    Task<RentalResponse> CompleteRentalAsync(int rentalId, int companyId);
    Task<RentalResponse> CancelRentalAsync(int rentalId, string reason);
}
