using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;

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

//only needed if we want to update review logs in bulk, otherwise we can just create new review log and add it to the review history
public class UpdateReviewHistoryCommand : ICommand
{
    public int ReviewHistoryId { get; set; }
    public List<ReviewLog> ReviewLogs { get; set; }

    public UpdateReviewHistoryCommand(int reviewHistoryId, List<ReviewLog> reviewLogs)
    {
        ReviewHistoryId = reviewHistoryId;
        ReviewLogs = reviewLogs;
    }
}

public class UpdateReviewHistoryCommandHandler : ICommandHandler<UpdateReviewHistoryCommand>
{
    private readonly IReviewHistoryRepository _reviewHistoryRepository;

    public UpdateReviewHistoryCommandHandler(IReviewHistoryRepository reviewHistoryRepository)
    {
        _reviewHistoryRepository = reviewHistoryRepository;
    }

    public async Task Handle(UpdateReviewHistoryCommand command)
    {
        var reviewHistory = await _reviewHistoryRepository.Get(command.ReviewHistoryId);
        if (reviewHistory == null)
        {
            throw new Exception("Review history not found");
        }
        reviewHistory.ReviewLogs = command.ReviewLogs;

        await _reviewHistoryRepository.Update(reviewHistory);
    }
}

public class DeleteReviewHistoryCommand : ICommand
{
    public int ReviewHistoryId { get; set; }

    public DeleteReviewHistoryCommand(int reviewHistoryId)
    {
        ReviewHistoryId = reviewHistoryId;
    }
}

public class DeleteReviewHistoryCommandHandler : ICommandHandler<DeleteReviewHistoryCommand>
{
    private readonly IReviewHistoryRepository _reviewHistoryRepository;

    public DeleteReviewHistoryCommandHandler(IReviewHistoryRepository reviewHistoryRepository)
    {
        _reviewHistoryRepository = reviewHistoryRepository;
    }

    public async Task Handle(DeleteReviewHistoryCommand command)
    {
        var reviewHistory = await _reviewHistoryRepository.Get(command.ReviewHistoryId);
        if (reviewHistory == null)
        {
            throw new Exception("Review history not found");
        }

        await _reviewHistoryRepository.Delete(command.ReviewHistoryId);
    }
}