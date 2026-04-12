using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;
using HazelNet_Infrastracture.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HazelNet_Infrastracture.DBServices.Repository;

//implementation of ideckrepository
public class DeckRepository : IDeckRepository
{
    private readonly ApplicationDbContext _context;

    public DeckRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Deck?> GetAsync(int deckId)
    {
        return await _context.Decks.FindAsync(deckId);
    }


    public async Task<List<Deck>> GetDeckByUserIdAsync(int userId)
    {
        return await _context.Decks
            .Where(d => d.UserId == userId)
            .ToListAsync();
    }
    public async Task UpdateAsync(Deck deck)
    {
        _context.Decks.Update(deck);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int deckId)
    {
        var deck = await GetAsync(deckId);
        if (deck != null)
        {
            _context.Decks.Remove(deck);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateAsync(Deck deck)
    {
        await _context.Decks.AddAsync(deck);
        await _context.SaveChangesAsync();
    }
}