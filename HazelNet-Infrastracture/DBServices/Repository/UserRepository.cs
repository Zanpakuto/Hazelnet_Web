using HazelNet_Application.Interface;
using HazelNet_Domain.Models;
using HazelNet_Infrastracture.DBContext;
using Microsoft.EntityFrameworkCore;

namespace HazelNet_Infrastracture.Command;

public class UserRepository :  IUserRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IPasswordHasher _passwordHasher;

    public UserRepository(
        IDbContextFactory<ApplicationDbContext> contextFactory, 
        IPasswordHasher passwordHasher)
    {
        _contextFactory = contextFactory;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> EmailExistAsync(string email)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        return await _context.User.AnyAsync(c => c.EmailAddress == email);
    }

    public async Task RegisterUserAsync(User user)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        _context.User.Add(user);
        await _context.SaveChangesAsync();
    }

    //Made it nullable for possiblity of null
    public async Task<string?> GetPasswordHashAsync(string email)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        return await _context.User
            .Where(c => c.EmailAddress == email)
            .Select(c => c.PasswordHash)
            .FirstOrDefaultAsync();
    }

    //Made a new query for getting user by email
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        return await _context.User
            .Where(c => c.EmailAddress == email)
            .FirstOrDefaultAsync();
    }
    
    public async Task DeleteUserByIdAsync(int userId)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        _context.User.Remove(_context.User.Find(userId));
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserNameAsync(int userId, string userName)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();

        var affected = await _context.User
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.Username, userName));

        if (affected == 0)
            throw new InvalidOperationException($"User {userId} not found.");
    }
    
    public async Task<string?> GetUsernameByUserIdAsync(int userId)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        return await _context.User
            .Where(u => u.Id == userId)
            .Select(u => u.Username)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdatePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();

        // 1. Retrieve the current hash
        var currentHash = await _context.User
            .Where(u => u.Id == userId)
            .Select(u => u.PasswordHash)
            .FirstOrDefaultAsync();

        // 2. Verify existence and validate current password using Argon2
        if (currentHash == null || !_passwordHasher.Verify(currentPassword, currentHash))
        {
            return false;
        }

        // 3. Hash the new password
        var newHash = _passwordHasher.Hash(newPassword);

        // 4. Update the hash directly
        var affected = await _context.User
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.PasswordHash, newHash));

        return affected > 0;
    }
}