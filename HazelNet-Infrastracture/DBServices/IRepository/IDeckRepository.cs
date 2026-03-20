using HazelNet_Domain.Models;

namespace HazelNet_Infrastracture.DBServices.Repositories;

public interface IDeckRepository
{
   Task<Deck?> Get(int deckId);
   Task Update(Deck deck);
    Task Delete(int deckId);
    Task Create(Deck deck);
}