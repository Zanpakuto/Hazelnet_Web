using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HazelNet_Domain.Models;
using HazelNet_Infrastracture.DBContext;

namespace HazelNet_Infrastracture.DBServices;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.User.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task AddUserAsync(User user)
    {
        _context.User.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.User.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.User.FindAsync(userId);
        if (user != null)
        {
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Deck>> GetDecksByUserIdAsync(int userId)
    {
        var user = await _context.User
            .Include(u => u.Decks) // Include related decks
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.Decks ?? new List<Deck>();
    }

    public async Task AddDeckToUserAsync(int userId, Deck deck)
    {
        var user = await _context.User
            .Include(u => u.Decks) // Include related decks
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null)
        {
            deck.UserId = userId; // Set the UserId for the deck
            deck.User = user; // Set the User navigation property
            user.Decks?.Add(deck);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteDecksByUserIdAsync(int userId)
    {
        var user = await _context.User
            .Include(u => u.Decks) // Include related decks
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null)
        {
            _context.Decks.RemoveRange(user.Decks ?? new List<Deck>());
            await _context.SaveChangesAsync();
        }
    }
}