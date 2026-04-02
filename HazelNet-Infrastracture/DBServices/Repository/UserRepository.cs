using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;
using HazelNet_Infrastracture.DBContext;
using Microsoft.EntityFrameworkCore;

namespace HazelNet_Infrastracture.DBServices.Repository;

//implementation of iuserrepository
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> Get(int userId)
    {
        return await _context.User.FindAsync(userId);
    }

    public async Task<User?> GetWithDecks(int userId)
    {
        return await _context.User
            .Include(u => u.Decks)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task Update(User user)
    {
        _context.User.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int userId)
    {
        var user = await Get(userId);
        if (user != null)
        {
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task Create(User user)
    {
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}