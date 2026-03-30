using HazelNet_Domain.Models;


namespace HazelNet_Domain.IRepository;


public interface IReviewHistoryRepository
{
    Task<ReviewHistory?> Get(int reviewHistoryId);
    Task<ReviewHistory?> GetReviewHistoryByCardId(int cardId);
    Task Update(ReviewHistory reviewHistory);
    Task Delete(int reviewHistoryId);
    Task Create(ReviewHistory reviewHistory);
}