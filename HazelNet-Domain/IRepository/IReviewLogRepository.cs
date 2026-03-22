using HazelNet_Domain.Models;

namespace HazelNet_Infrastracture.DBServices.IRepository;

public interface IReviewLogRepository
{
    Task<ReviewLog?> Get(int reviewLogId);
    Task Update(ReviewLog reviewLog);
    Task Delete(int reviewLogId);
    Task Create(ReviewLog reviewLog);
}