using HazelNet_Application.Interface;
using HazelNet_Domain.Models;
using HazelNet_Infrastracture.DBContext;
using Microsoft.EntityFrameworkCore;

namespace HazelNet_Infrastracture.Command;

public class UserRepository :  IUserRepository
{
    public readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EmailExistAsync(string email)
    {
        return await _context.User.AnyAsync(c => c.EmailAddress == email);
    }

    public async Task RegisterUserAsync(User user)
    {
        _context.User.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<string> GetPasswordHashAsync(string email)
    {
        return await _context.User
            .Where(c => c.EmailAddress == email)
            .Select(c => c.PasswordHash)
            .FirstOrDefaultAsync();
    }
}