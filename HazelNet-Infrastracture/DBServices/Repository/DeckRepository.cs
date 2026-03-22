using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;
using HazelNet_Infrastracture.DBContext;
using Microsoft.EntityFrameworkCore;

namespace HazelNet_Infrastracture.DBServices.Repository;
public class DeckRepository : IDeckRepository
{
    private readonly ApplicationDbContext _context;

    public DeckRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Deck?> Get(int deckId)
    {
        return await _context.Decks.FindAsync(deckId);
    }

    public async Task Update(Deck deck)
    {
        _context.Decks.Update(deck);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int deckId)
    {
        var deck = await Get(deckId);
        if (deck != null)
        {
            _context.Decks.Remove(deck);
            await _context.SaveChangesAsync();
        }
    }

    public async Task Create(Deck deck)
    {
        await _context.Decks.AddAsync(deck);
        await _context.SaveChangesAsync();
    }
}