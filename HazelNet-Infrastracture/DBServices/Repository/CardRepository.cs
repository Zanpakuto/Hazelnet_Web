using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;
using HazelNet_Infrastracture.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HazelNet_Infrastracture.DBServices.Repository;

//implementation of icardrepository 
public class CardRepository : ICardRepository
{
    private readonly ApplicationDbContext _context;

    public CardRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Card?> Get(int cardId)
    {
        return await _context.Cards.FindAsync(cardId);
    }

    public async Task<List<Card>> GetCardByDeckId(int deckId)
    {
        return await _context.Cards
            .Where(c => c.DeckId == deckId)
            .ToListAsync();
    }

    public async Task Update(Card card)
    {
        _context.Cards.Update(card);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int cardId)
    {
        var card = await Get(cardId);
        if (card != null)
        {
            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
        }
    }

    public async Task Create(Card card)
    {
        await _context.Cards.AddAsync(card);
        await _context.SaveChangesAsync();
    }
}