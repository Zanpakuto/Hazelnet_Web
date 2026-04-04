using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;
using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Queries;

/*
    HOW TO USE
    1. Inject the query handler into your controller or service where you want to execute the query
    ex:
    private readonly GetCardByIdQueryHandler _getCardByIdQueryHandler;
    public YourController(GetCardByIdQueryHandler getCardByIdQueryHandler)
    {
        _getCardByIdQueryHandler = getCardByIdQueryHandler;
    }
    2. Create an instance of the query with the required parameters and execute it using the handler
    ex:
    var query = new GetCardByIdQuery(cardId);
    var card = await _getCardByIdQueryHandler.Handle(query);
    3. Query handler will execute the logic defined and use repositories to interact with the database as needed
*/

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