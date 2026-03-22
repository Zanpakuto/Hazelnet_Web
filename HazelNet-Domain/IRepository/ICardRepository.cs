using HazelNet_Domain.Models;

namespace HazelNet_Domain.IRepository;

public interface ICardRepository
{
    Task<Card?> Get(int cardId);
    Task Update(Card card);
    Task Delete(int cardId);
    Task Create(Card card);
}