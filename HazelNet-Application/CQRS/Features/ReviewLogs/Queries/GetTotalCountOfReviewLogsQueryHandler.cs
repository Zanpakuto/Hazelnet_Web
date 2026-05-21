using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Application.CQRS.Abstractions.Identity;
using HazelNet_Domain.IRepository;

namespace HazelNet_Application.CQRS.Features.ReviewLogs.Queries;

public class GetTotalCountOfReviewLogsQueryHandler : IQueryHandler<GetTotalCountOfReviewLogsQuery, int>
{
    private readonly IReviewLogRepository  _repository;
    private readonly ICurrentUserService _currentUserService;
    
    public GetTotalCountOfReviewLogsQueryHandler(IReviewLogRepository repository,  ICurrentUserService currentUserService)
    {    
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<int> Handle(GetTotalCountOfReviewLogsQuery query)
    {
        var userIdString = await _currentUserService.GetUserIdAsync();
        
        if (!int.TryParse(userIdString, out var userId))
        {
            throw new Exception("User is not authenticated or token is invalid.");
        }
        
        if (query.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this deck.");
        }
        
        return await _repository.GetTotalReviewsByUserIdAsync(query.UserId);
    }
}