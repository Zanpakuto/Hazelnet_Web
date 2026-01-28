using HazelNet_Domain.Models;

namespace HazelNet_Domain.Services.FSRS;

public interface IImplScheduler
{
    SchedulingInfo NewState(Rating grade);
    SchedulingInfo LearningState(Rating grade);
    SchedulingInfo ReviewState(Rating grade);
}