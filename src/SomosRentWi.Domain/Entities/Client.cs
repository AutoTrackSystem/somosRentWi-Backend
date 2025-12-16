using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Domain.Entities;

public class Client
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public ClientDocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public string PrimaryPhone { get; set; } = string.Empty;
    public string SecondaryPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public ClientVerificationStatus VerificationStatus { get; set; } = ClientVerificationStatus.Pending;
    public ClientHistoryStatus HistoryStatus { get; set; } = ClientHistoryStatus.NoHistory;
    
    public string UrlSelfiePhoto { get; set; } = string.Empty; // Selfie Photo
    public string UrlDocumentFront { get; set; } = string.Empty; // Document Front
    public string UrlDocumentBack { get; set; } = string.Empty; // Document Back
    public string UrlLicenseFront { get; set; } = string.Empty; // License Front
    public string UrlLicenseBack { get; set; } = string.Empty; // License Back
}