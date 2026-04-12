using HazelNet_Domain.Models;


namespace HazelNet_Domain.IRepository;

public interface IDeckRepository
{
   Task<Deck?> GetAsync(int deckId);
    Task<List<Deck>> GetDeckByUserIdAsync(int userId);
   Task UpdateAsync(Deck deck);
    Task DeleteAsync(int deckId);
    Task CreateAsync(Deck deck);
}