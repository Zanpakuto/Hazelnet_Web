using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HazelNet_Domain.Models;
using HazelNet_Infrastracture.DBContext;

namespace HazelNet_Infrastracture.DBServices;

public class DeckService
{
    private readonly ApplicationDbContext _context;

    public DeckService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Deck>> GetAllDecksAsync()
    {
        return await _context.Decks.ToListAsync();
    }

    public async Task<Deck> GetDeckByIdAsync(int deckId)
    {
        return await _context.Decks
            .Include(d => d.Cards) // Include related cards
            .FirstOrDefaultAsync(d => d.Id == deckId);
    }

    public async Task AddDeckAsync(Deck deck)
    {
        _context.Decks.Add(deck);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateDeckAsync(Deck deck)
    {
        var existingDeck = await _context.Decks.FindAsync(deck.Id);
        if (existingDeck != null)
        {
            existingDeck.DeckName = deck.DeckName;
            existingDeck.LastAcess = DateTime.Now; 
            _context.Decks.Update(existingDeck);
            await _context.SaveChangesAsync();
        }

    }

    public async Task DeleteDeckAsync(int deckId)
    {
        var deck = await _context.Decks.FindAsync(deckId);
        if (deck != null)
        {
            _context.Decks.Remove(deck);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Card>> GetCardsByDeckIdAsync(int deckId)
    {
        return await _context.Cards
            .Where(c => c.DeckId == deckId)
            .ToListAsync();
    }

    public async Task AddCardToDeckAsync(int deckId, Card card)
    {
        var deck = await _context.Decks
            .Include(d => d.Cards)
            .FirstOrDefaultAsync(d => d.Id == deckId);

        if (deck != null)
        {
            card.DeckId = deckId;
            card.Deck = deck;
            deck.Cards.Add(card);
            deck.LastAcess = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }   

    public async Task RemoveCardFromDeckAsync(int deckId, int cardId)
    {
        var card = await _context.Cards
            .FirstOrDefaultAsync(c => c.Id == cardId && c.DeckId == deckId);

        if (card != null)
        {
            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
        }
    }
}