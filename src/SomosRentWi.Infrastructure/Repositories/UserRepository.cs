using Microsoft.EntityFrameworkCore;
using SomosRentWi.Domain.Entities;
using SomosRentWi.Domain.IRepositories;
using SomosRentWi.Infrastructure.Persistence;

namespace SomosRentWi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly RentWiDbContext _context;

    public UserRepository(RentWiDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalized = email.Trim().ToLower();
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == normalized);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var normalized = email.Trim().ToLower();
        return await _context.Users.AnyAsync(u => u.Email == normalized);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}