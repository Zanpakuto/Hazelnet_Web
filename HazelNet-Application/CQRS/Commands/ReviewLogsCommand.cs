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
public class CreateReviewLogCommand : ICommand
{
    public int ReviewHistoryId { get; set; }
    public Rating Rating { get; set; }
    public ulong ScheduledDays { get; set; }
    public ulong ElapsedDays { get; set; }
    public DateTime Review { get; set; }
    public State State { get; set; }

    public CreateReviewLogCommand(int reviewHistoryId, Rating rating, ulong scheduledDays, ulong elapsedDays, DateTime review, State state)
    {
        ReviewHistoryId = reviewHistoryId;
        Rating = rating;
        ScheduledDays = scheduledDays;
        ElapsedDays = elapsedDays;
        Review = review;
        State = state;
    }
}

public class CreateReviewLogCommandHandler : ICommandHandler<CreateReviewLogCommand>
{
    private readonly IReviewLogRepository _reviewLogRepository;
    private readonly IReviewHistoryRepository _reviewHistoryRepository;

    public CreateReviewLogCommandHandler(IReviewLogRepository reviewLogRepository, IReviewHistoryRepository reviewHistoryRepository)
    {
        _reviewLogRepository = reviewLogRepository;
        _reviewHistoryRepository = reviewHistoryRepository;
    }

    public async Task Handle(CreateReviewLogCommand command)
    {
        var reviewHistory = await _reviewHistoryRepository.Get(command.ReviewHistoryId);
        if (reviewHistory == null)
        {
            throw new Exception("Review history not found");
        }
        var reviewLog = new ReviewLog
        {
            ReviewHistoryId = command.ReviewHistoryId,
            Rating = command.Rating,
            ScheduledDays = command.ScheduledDays,
            ElapsedDays = command.ElapsedDays,
            Review = command.Review,
            State = command.State
        };

        await _reviewLogRepository.Create(reviewLog);
    }
}

public class UpdateReviewLogCommand : ICommand
{
    public int ReviewLogId { get; set; }
    public Rating Rating { get; set; }
    public ulong ScheduledDays { get; set; }
    public ulong ElapsedDays { get; set; }
    public DateTime Review { get; set; }
    public State State { get; set; }

    public UpdateReviewLogCommand(int reviewLogId, Rating rating, ulong scheduledDays, ulong elapsedDays, DateTime review, State state)
    {
        ReviewLogId = reviewLogId;
        Rating = rating;
        ScheduledDays = scheduledDays;
        ElapsedDays = elapsedDays;
        Review = review;
        State = state;
    }
}

public class UpdateReviewLogCommandHandler : ICommandHandler<UpdateReviewLogCommand>
{
    private readonly IReviewLogRepository _reviewLogRepository;

    public UpdateReviewLogCommandHandler(IReviewLogRepository reviewLogRepository)
    {
        _reviewLogRepository = reviewLogRepository;
    }

    public async Task Handle(UpdateReviewLogCommand command)
    {
        var reviewLog = await _reviewLogRepository.Get(command.ReviewLogId);
        if (reviewLog == null)
        {
            throw new Exception("Review log not found");
        }
        reviewLog.Rating = command.Rating;
        reviewLog.ScheduledDays = command.ScheduledDays;
        reviewLog.ElapsedDays = command.ElapsedDays;
        reviewLog.Review = command.Review;
        reviewLog.State = command.State;

        await _reviewLogRepository.Update(reviewLog);
    }
}

public class DeleteReviewLogCommand : ICommand
{
    public int ReviewLogId { get; set; }

    public DeleteReviewLogCommand(int reviewLogId)
    {
        ReviewLogId = reviewLogId;
    }
}

public class DeleteReviewLogCommandHandler : ICommandHandler<DeleteReviewLogCommand>
{
    private readonly IReviewLogRepository _reviewLogRepository;

    public DeleteReviewLogCommandHandler(IReviewLogRepository reviewLogRepository)
    {
        _reviewLogRepository = reviewLogRepository;
    }

    public async Task Handle(DeleteReviewLogCommand command)
    {
        var reviewLog = await _reviewLogRepository.Get(command.ReviewLogId);
        if (reviewLog == null)
        {
            throw new Exception("Review log not found");
        }

        await _reviewLogRepository.Delete(command.ReviewLogId);
    }
}