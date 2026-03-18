using HazelNet_Domain.Models;

namespace HazelNet_Infrastracture.DBServices.Repositories;

public interface IDeckRepository
{
    Task<List<Deck>> GetAllDecksAsync();
    Task<Deck?> GetDeckByIdAsync(int deckId);
    Task UpdateDeckAsync(Deck deck);

    Task AddCardToDeckAsync(int deckId, Card card);
    Task<List<Card>> GetCardsByDeckIdAsync(int deckId);
    Task DeleteCardFromDeckAsync(int deckId, int cardId);
}