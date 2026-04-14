using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;
using HazelNet_Web.ViewModel;

namespace HazelNet_Application.CQRS.Features.Decks.Queries;

public class GetDecksQueryHandler 
    : IQueryHandler<GetDecksQuery, List<DeckViewModel>>
{
    private readonly IDeckRepository _deckRepository;
    public GetDecksQueryHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }
        
    public async Task<List<DeckViewModel>> Handle(GetDecksQuery query)
    {

        var decks = await _deckRepository.GetDeckByUserIdAsync(query.UserId);


        var result = decks.Select(d => new DeckViewModel
        {
            Name = d.DeckName,
            Description = d.DeckDescription,
            TotalNumberOfCards = d.Cards.Count,
            CreationDate = d.CreationDate,
            LastDateAccessed = d.LastAcess
        }).ToList();

        return result;
    }
}

