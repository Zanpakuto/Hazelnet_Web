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

    //Retrieves all decks
    public async Task<List<Deck>> GetAllDecksAsync()
    {
        return await _context.Decks.ToListAsync();
    }

    //retrieve a deck by ID
    public async Task<Deck?> GetDeckByIdAsync(int deckId)
    {
        return await _context.Decks
            .Include(d => d.Cards) // Include related cards
            .FirstOrDefaultAsync(d => d.Id == deckId);
    }

   

    //call wheneved deck is modified
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

 

    //retrieves all cards in a deck
    public async Task<List<Card>> GetCardsByDeckIdAsync(int deckId)
    {
        return await _context.Cards
            .Where(c => c.DeckId == deckId)
            .ToListAsync();
    }

    //adds a card to a deck. 
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

    //removes a card from a deck. use this method instead of CardService method to maintain referential integrity and update deck's last access time
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