using HazelNet_Application.DBServices.Abstractions;
using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;

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