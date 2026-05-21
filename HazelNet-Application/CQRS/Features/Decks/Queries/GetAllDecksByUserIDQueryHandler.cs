using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Application.CQRS.Abstractions.Identity;
using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;

namespace HazelNet_Application.CQRS.Features.Decks.Queries;

public class GetAllDecksByUserIDQueryHandler : IQueryHandler<GetAllDecksByUserIDQuery, List<Deck>>
{
    private readonly IDeckRepository _deckRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllDecksByUserIDQueryHandler(IDeckRepository deckRepository, ICurrentUserService currentUserService)
    {
        _deckRepository = deckRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<Deck>> Handle(GetAllDecksByUserIDQuery query)
    {
        var userIdString = await _currentUserService.GetUserIdAsync();
        
        if (!int.TryParse(userIdString, out var userId))
        {
            throw new Exception("User is not authenticated or token is invalid.");
        }
        
        
        var deck =  await _deckRepository.GetAllDeckByUserIdAsync(query.UserId);
        
        if (deck is null)
            throw new Exception($"User has no deck with userId {query.UserId}");
        
        if (query.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to this deck.");
        }
        
         return deck;
    }
}