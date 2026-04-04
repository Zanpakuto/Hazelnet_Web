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

public class GetDeckByIdQuery : IQuery<Deck>
{
    public int DeckId { get; set; }

    public GetDeckByIdQuery(int deckId)
    {
        DeckId = deckId;
    }
}

public class GetDeckByIdQueryHandler : IQueryHandler<GetDeckByIdQuery, Deck>
{
    private readonly IDeckRepository _deckRepository;

    public GetDeckByIdQueryHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    public async Task<Deck> Handle(GetDeckByIdQuery query)
    {
        var deck = await _deckRepository.Get(query.DeckId);
        return deck ?? throw new InvalidOperationException($"Deck with id {query.DeckId} not found.");
    }
}

public class GetDecksByUserIdQuery : IQuery<List<Deck>>
{
    public int UserId { get; set; }

    public GetDecksByUserIdQuery(int userId)
    {
        UserId = userId;
    }
}

public class GetDecksByUserIdQueryHandler : IQueryHandler<GetDecksByUserIdQuery, List<Deck>>
{
    private readonly IDeckRepository _deckRepository;

    public GetDecksByUserIdQueryHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    public async Task<List<Deck>> Handle(GetDecksByUserIdQuery query)
    {
        return await _deckRepository.GetDeckByUserId(query.UserId);
    }
}