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

    //retrieves all users
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.User.ToListAsync();
    }

    //retrieves a user by ID
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    //adds a new user to the database
    public async Task AddUserAsync(User user)
    {
        _context.User.Add(user);
        await _context.SaveChangesAsync();
    }

    //updates the user whenever contents are modified
    public async Task UpdateUserAsync(User user)
    {
        _context.User.Update(user);
        await _context.SaveChangesAsync();
    }

    //deletes a user from the database
    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.User.FindAsync(userId);
        if (user != null)
        {
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    //retrieves all decks associated with a user
    public async Task<List<Deck>> GetDecksByUserIdAsync(int userId)
    {
        var user = await _context.User
            .Include(u => u.Decks) // Include related decks
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.Decks ?? new List<Deck>();
    }

    //adds a deck to a user
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

    //deletes all decks associated with a user
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