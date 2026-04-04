using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;

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
public class GetCardByIdQuery : IQuery<Card>
{
    public int Id { get; set; }
    public GetCardByIdQuery(int id)
    {
        Id = id;
    }
}

public class GetCardByIdQueryHandler : IQueryHandler<GetCardByIdQuery, Card>
{
    private readonly ICardRepository _cardRepository;
    public GetCardByIdQueryHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Card> Handle(GetCardByIdQuery query)
    {
        var card = await _cardRepository.Get(query.Id);
        return card ?? throw new InvalidOperationException($"Card with id {query.Id} not found.");
    }
}

public class GetCardsByDeckIdQuery : IQuery<List<Card>>
{
    public int DeckId { get; set; }
    public GetCardsByDeckIdQuery(int deckId)
    {
        DeckId = deckId;
    }
}

public class GetCardsByDeckIdQueryHandler : IQueryHandler<GetCardsByDeckIdQuery, List<Card>>
{
    private readonly ICardRepository _cardRepository;
    public GetCardsByDeckIdQueryHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<List<Card>> Handle(GetCardsByDeckIdQuery query)
    {
        return await _cardRepository.GetCardByDeckId(query.DeckId);
    }
}