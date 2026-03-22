using HazelNet_Domain.Models;

namespace HazelNet_Infrastracture.DBServices.IRepository;

public interface IReviewHistoryRepository
{
    Task<ReviewHistory?> Get(int reviewHistoryId);
    Task Update(ReviewHistory reviewHistory);
    Task Delete(int reviewHistoryId);
    Task Create(ReviewHistory reviewHistory);
}