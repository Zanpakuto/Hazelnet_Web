using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;
using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Commands;

public class CreateDeckCommand : ICommand
{
    public string Name { get; set; }
    public int UserId { get; set; }

    public CreateDeckCommand(string name, int userId)
    {
        Name = name;
        UserId = userId;
    }
}

public class CreateDeckCommandHandler : ICommandHandler<CreateDeckCommand>
{
    private readonly IDeckRepository _deckRepository;
    private readonly IUserRepository _userRepository;

    public CreateDeckCommandHandler(IDeckRepository deckRepository, IUserRepository userRepository)
    {
        _deckRepository = deckRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(CreateDeckCommand command)
    {
        var user = await _userRepository.Get(command.UserId);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        var deck = new Deck
        {
            DeckName = command.Name,
            UserId = command.UserId
        };

        await _deckRepository.Create(deck);
    }
}

public class UpdateDeckCommand : ICommand
{
    public int DeckId { get; set; }
    public string Name { get; set; }

    public UpdateDeckCommand(int deckId, string name)
    {
        DeckId = deckId;
        Name = name;
    }
}

public class UpdateDeckCommandHandler : ICommandHandler<UpdateDeckCommand>
{
    private readonly IDeckRepository _deckRepository;

    public UpdateDeckCommandHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    public async Task Handle(UpdateDeckCommand command)
    {
        var deck = await _deckRepository.Get(command.DeckId);
        if (deck == null)
        {
            throw new Exception("Deck not found");
        }
        deck.DeckName = command.Name;
        await _deckRepository.Update(deck);
    }
}

public class DeleteDeckCommand : ICommand
{
    public int DeckId { get; set; }

    public DeleteDeckCommand(int deckId)
    {
        DeckId = deckId;
    }
}

public class DeleteDeckCommandHandler : ICommandHandler<DeleteDeckCommand>
{
    private readonly IDeckRepository _deckRepository;

    public DeleteDeckCommandHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    public async Task Handle(DeleteDeckCommand command)
    {
        await _deckRepository.Delete(command.DeckId);
    }
}