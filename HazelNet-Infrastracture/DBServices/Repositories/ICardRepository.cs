using HazelNet_Domain.Models;

namespace HazelNet_Infrastracture.DBServices.Repositories;

public interface ICardRepository
{
    Task<List<Card>> GetAllCardsAsync();
    Task<Card?> GetCardByIdAsync(int cardId);
    Task UpdateCardAsync(Card card);
    Task<ReviewHistory?> GetReviewHistoryByCardIdAsync(int cardId);
    Task AddReviewHistoryToCardAsync(int cardId, ReviewHistory reviewHistory);
}