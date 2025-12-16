using Microsoft.AspNetCore.Http;
using SomosRentWi.Domain.Enums;

namespace SomosRentWi.Application.Auth.DTOs;

public class RegisterClientRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public ClientDocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public string PrimaryPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    
    // Photo uploads (all required)
    public IFormFile? SelfiePhoto { get; set; }
    public IFormFile? DocumentFront { get; set; }
    public IFormFile? DocumentBack { get; set; }
    public IFormFile? LicenseFront { get; set; }
    public IFormFile? LicenseBack { get; set; }
}