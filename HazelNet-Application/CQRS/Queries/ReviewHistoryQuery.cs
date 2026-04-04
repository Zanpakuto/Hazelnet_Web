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

//query to get review history by id
public class GetReviewHistoryByIdQuery : IQuery<ReviewHistory?>
{
    public int ReviewHistoryId { get; set; }
    public GetReviewHistoryByIdQuery(int reviewHistoryId)
    {
        ReviewHistoryId = reviewHistoryId;
    }
}

//query handler to get review history by id
public class GetReviewHistoryByIdQueryHandler : IQueryHandler<GetReviewHistoryByIdQuery, ReviewHistory?>
{
    private readonly IReviewHistoryRepository _reviewHistoryRepository;

    public GetReviewHistoryByIdQueryHandler(IReviewHistoryRepository reviewHistoryRepository)
    {
        _reviewHistoryRepository = reviewHistoryRepository;
    }

    public async Task<ReviewHistory?> Handle(GetReviewHistoryByIdQuery query)
    {
        return await _reviewHistoryRepository.Get(query.ReviewHistoryId);
    }
}

//query to get review history by card id
public class GetReviewHistoryByCardIdQuery : IQuery<ReviewHistory?>
{
    public int CardId { get; set; }
    public GetReviewHistoryByCardIdQuery(int cardId)
    {
        CardId = cardId;
    }
}

//query handler to get review history by card id
public class GetReviewHistoryByCardIdQueryHandler : IQueryHandler<GetReviewHistoryByCardIdQuery, ReviewHistory?>
{
    private readonly IReviewHistoryRepository _reviewHistoryRepository;

    public GetReviewHistoryByCardIdQueryHandler(IReviewHistoryRepository reviewHistoryRepository)
    {
        _reviewHistoryRepository = reviewHistoryRepository;
    }

    public async Task<ReviewHistory?> Handle(GetReviewHistoryByCardIdQuery query)
    {
        return await _reviewHistoryRepository.GetReviewHistoryByCardId(query.CardId);
    }
}

