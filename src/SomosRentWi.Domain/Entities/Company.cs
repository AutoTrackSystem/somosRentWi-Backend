using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Domain.Entities;

public class Company
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public string TradeName { get; set; } = string.Empty;
    public string NitNumber { get; set; } = string.Empty;
    
    public string ContactEmail { get; set; } = string.Empty;
    public string LandlineNumber { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    public CompanyPlan CompanyPlan { get; set; } = CompanyPlan.None;
    public CompanySubscriptionStatus SubscriptionStatus { get; set; } = CompanySubscriptionStatus.Inactive;
    
    public DateTime? StartSubscriptionDate { get; set; }
    public DateTime? EndSubscriptionDate { get; set; }
    
    public CompanyWallet? Wallet { get; set; }
    public ICollection<Car> Cars { get; set; } = new List<Car>();
}