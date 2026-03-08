using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HazelNet_Domain.Models;
using HazelNet_Infrastracture.DBContext;

namespace HazelNet_Infrastracture.DBServices;

public class CardService
{
    private readonly ApplicationDbContext _context;

    public CardService(ApplicationDbContext context)
    {
        _context = context;
    }

    //Retrieves all cards
    public async Task<List<Card>> GetAllCardsAsync()
    {
        return await _context.Cards.ToListAsync();
    }


    //retrieve a card by ID
    public async Task<Card?> GetCardByIdAsync(int cardId)
    {
        return await _context.Cards
            .FirstOrDefaultAsync(c => c.Id == cardId);
    }


    //update the card whenever contents are modified
    public async Task UpdateCardAsync(Card card)
    {
        _context.Cards.Update(card);
        await _context.SaveChangesAsync();
    }

    //retrieves review history for a card
    public async Task<ReviewHistory?> GetReviewHistoryByCardIdAsync(int cardId)
    {
        return await _context.ReviewHistory
            .Include(rh => rh.ReviewLogs) // Include related review logs
            .FirstOrDefaultAsync(rh => rh.CardId == cardId);
    }

    //adds review history to a card. sets reviewhistory ID to be the same as card ID to maintain 1:1 relationship
    public async Task AddReviewHistoryToCardAsync(int cardId, ReviewHistory reviewHistory)
    {
        var card = await _context.Cards.FindAsync(cardId);
        if (card != null)
        {
            reviewHistory.Id = cardId; //1:1 
            reviewHistory.CardId = cardId; // Set the foreign key
            reviewHistory.Card = card; // Set the navigation property
            _context.ReviewHistory.Add(reviewHistory);
            await _context.SaveChangesAsync();
        }
    }

    //deletes review history for a card. recommend using this method instead of ReviewHistoryService method to maintain referential integrity
    public async Task DeleteReviewHistoryByCardIdAsync(int cardId)
    {
        var reviewHistory = await _context.ReviewHistory
            .FirstOrDefaultAsync(rh => rh.CardId == cardId);

        if (reviewHistory != null)
        {
            _context.ReviewHistory.Remove(reviewHistory);
            await _context.SaveChangesAsync();
        }
    }

}