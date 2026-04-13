using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;

namespace HazelNet_Application.CQRS.Features.Decks.Commands;

public class CreateDeckCommandHandler : ICommandHandler<CreateDeckCommand, int>
{
    private IDeckRepository _deckRepository;

    public CreateDeckCommandHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }


    public async Task<int> Handle(CreateDeckCommand command)
    {
        var deck = new Deck
        {
            DeckName = command.DeckName,
            DeckDescription = command.DeckDescription,
            CreationDate = DateTime.UtcNow,
            LastAcess = DateTime.UtcNow,
            UserId = command.UserId,
        };
        
        await _deckRepository.CreateAsync(deck);
        return deck.Id;
    }
}