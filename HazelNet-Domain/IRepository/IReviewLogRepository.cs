using HazelNet_Domain.Models;


namespace HazelNet_Domain.IRepository;


public interface IReviewLogRepository
{
    Task<ReviewLog?> GetReviewLogByIdAsync(int reviewLogId);
    Task<List<ReviewLog>> GetReviewLogsByReviewHistoryIdAsync(int reviewHistoryId);
    Task<IReadOnlyDictionary<int, List<ReviewLog>>> GetReviewLogsByHistoryIdsAsync(IEnumerable<int> historyIds);

    Task<int> GetTotalReviewsByUserIdAsync(int userId);
    Task UpdateAsync(ReviewLog reviewLog);
    Task DeleteAsync(int reviewLogId);
    Task CreateAsync(ReviewLog reviewLog);
}