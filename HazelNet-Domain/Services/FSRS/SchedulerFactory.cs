using HazelNet_Domain.Models;

namespace HazelNet_Domain.Services.FSRS;

public static class SchedulerFactory
{
    //selects between short-term and long-term scheduler based on parameters
    public static Scheduler SchedulerFor(Parameters p, Card card, DateTime now)
    {
        if (p.EnableShortTerm)
            return Scheduler.NewScheduler(p, card, now, s => new BasicScheduler(s));
        else
            return Scheduler.NewScheduler(p, card, now, s => new LongTermScheduler(s));
    }
}