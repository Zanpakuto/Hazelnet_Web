using HazelNet_Domain.Models;


namespace HazelNet_Domain.IRepository;

public interface IDeckRepository
{
   Task<Deck?> Get(int deckId);
   Task Update(Deck deck);
    Task Delete(int deckId);
    Task Create(Deck deck);
}