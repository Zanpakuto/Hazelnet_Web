using HazelNet_Domain.Models;


namespace HazelNet_Domain.IRepository;


public interface IReviewLogRepository
{
    Task<ReviewLog?> Get(int reviewLogId);
    Task<List<ReviewLog>> GetReviewLogsByReviewHistoryId(int reviewHistoryId);
    Task Update(ReviewLog reviewLog);
    Task Delete(int reviewLogId);
    Task Create(ReviewLog reviewLog);
}