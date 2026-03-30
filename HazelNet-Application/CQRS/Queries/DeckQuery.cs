using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;
using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Queries;

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