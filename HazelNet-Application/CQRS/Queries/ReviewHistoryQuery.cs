using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;
using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Queries;

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

