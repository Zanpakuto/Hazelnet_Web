using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;

namespace HazelNet_Application.CQRS.Commands;

public class CreateReviewHistoryCommand : ICommand
{
    public int CardId { get; set; }

    public CreateReviewHistoryCommand(int cardId)
    {
        CardId = cardId;
    }
}

public class CreateReviewHistoryCommandHandler : ICommandHandler<CreateReviewHistoryCommand>
{
    private readonly IReviewHistoryRepository _reviewHistoryRepository;
    private readonly ICardRepository _cardRepository;

    public CreateReviewHistoryCommandHandler(IReviewHistoryRepository reviewHistoryRepository, ICardRepository cardRepository)
    {
        _reviewHistoryRepository = reviewHistoryRepository;
        _cardRepository = cardRepository;
    }

    public async Task Handle(CreateReviewHistoryCommand command)
    {
        var card = await _cardRepository.Get(command.CardId);
        if (card == null)
        {
            throw new Exception("Card not found");
        }
        var reviewHistory = new ReviewHistory(command.CardId);

        await _reviewHistoryRepository.Create(reviewHistory);
    }
}