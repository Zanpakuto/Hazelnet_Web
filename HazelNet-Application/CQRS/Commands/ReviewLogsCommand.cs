using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;

namespace HazelNet_Application.CQRS.Commands;

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