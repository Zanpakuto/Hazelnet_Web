using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;
using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Queries;

//query to get review logs by id
public class GetReviewLogsByIdQuery : IQuery<ReviewLog?>
{
    public int ReviewLogId { get; set; }
    public GetReviewLogsByIdQuery(int reviewLogId)
    {
        ReviewLogId = reviewLogId;
    }
}

//query handler to get review logs by id
public class GetReviewLogsByIdQueryHandler : IQueryHandler<GetReviewLogsByIdQuery, ReviewLog?>
{
    private readonly IReviewLogRepository _reviewLogRepository;

    public GetReviewLogsByIdQueryHandler(IReviewLogRepository reviewLogRepository)
    {
        _reviewLogRepository = reviewLogRepository;
    }

    public async Task<ReviewLog?> Handle(GetReviewLogsByIdQuery query)
    {
        return await _reviewLogRepository.Get(query.ReviewLogId);
    }
}

//query to get review logs by review history id
public class GetReviewLogsByReviewHistoryIdQuery : IQuery<List<ReviewLog>>
{
    public int ReviewHistoryId { get; set; }
    public GetReviewLogsByReviewHistoryIdQuery(int reviewHistoryId)
    {
        ReviewHistoryId = reviewHistoryId;
    }
}

//query handler to get review logs by review history id
public class GetReviewLogsByReviewHistoryIdQueryHandler : IQueryHandler<GetReviewLogsByReviewHistoryIdQuery, List<ReviewLog>>
{
    private readonly IReviewLogRepository _reviewLogRepository;

    public GetReviewLogsByReviewHistoryIdQueryHandler(IReviewLogRepository reviewLogRepository)
    {
        _reviewLogRepository = reviewLogRepository;
    }

    public async Task<List<ReviewLog>> Handle(GetReviewLogsByReviewHistoryIdQuery query)
    {
        return await _reviewLogRepository.GetReviewLogsByReviewHistoryId(query.ReviewHistoryId);
    }
}