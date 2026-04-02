using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;
using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Commands;

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