using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;
using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Commands;

/*
    HOW TO USE
    1. Inject the command handler into your controller or service where you want to execute the command
    ex:
    private readonly CreateCardCommandHandler _createCardCommandHandler;
    public YourController(CreateCardCommandHandler createCardCommandHandler)
    {
        _createCardCommandHandler = createCardCommandHandler;
    }

    2. Create an instance of the command with the required parameters and execute it using the handler
    ex:
    var command = new CreateCardCommand(deckId, front, back);
    await _createCardCommandHandler.Handle(command);

    3. Command handler will execute the logic defined and use repositories to interact with the database as needed
    
*/


public class CreateCardCommand : ICommand
{
    public int DeckId { get; set; }
    public string Front { get; set; }
    public string Back { get; set; }

    public CreateCardCommand(int deckId, string front, string back)
    {
        DeckId = deckId;
        Front = front;
        Back = back;
    }
}
public class CreateCardCommandHandler : ICommandHandler<CreateCardCommand>
{
    private readonly ICardRepository _cardRepository;
    private readonly IReviewHistoryRepository _reviewHistoryRepository;

    public CreateCardCommandHandler(ICardRepository cardRepository, IReviewHistoryRepository reviewHistoryRepository)
    {
        _cardRepository = cardRepository;
        _reviewHistoryRepository = reviewHistoryRepository;
    }

    public async Task Handle(CreateCardCommand command)
    {
       
        var card = new Card
        {
            DeckId = command.DeckId,
            FrontOfCard = command.Front,
            BackOfCard = command.Back
        };

        var reviewHistory = new ReviewHistory(card.Id);

        await _reviewHistoryRepository.Create(reviewHistory);
        await _cardRepository.Create(card);
    }
}

public class UpdateCardCommand : ICommand
{
    public int CardId { get; set; }
    public string Front { get; set; }
    public string Back { get; set; }

    public UpdateCardCommand(int cardId, string front, string back)
    {
        CardId = cardId;
        Front = front;
        Back = back;
    }
}

public class UpdateCardCommandHandler : ICommandHandler<UpdateCardCommand>
{
    private readonly ICardRepository _cardRepository;

    public UpdateCardCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task Handle(UpdateCardCommand command)
    {
        var card = await _cardRepository.Get(command.CardId);
        if (card != null)
        {
            card.FrontOfCard = command.Front;
            card.BackOfCard = command.Back;
            await _cardRepository.Update(card);
        }
    }
}

public class DeleteCardCommand : ICommand
{
    public int CardId { get; set; }

    public DeleteCardCommand(int cardId)
    {
        CardId = cardId;
    }
}

public class DeleteCardCommandHandler : ICommandHandler<DeleteCardCommand>
{
    private readonly ICardRepository _cardRepository;

    public DeleteCardCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task Handle(DeleteCardCommand command)
    {
        await _cardRepository.Delete(command.CardId);
    }
}