using HazelNet_Domain.Models;

namespace HazelNet_Domain.Services.FSRS;

public class FSRS
{
    public Parameters Parameters { get; }

    public FSRS(Parameters parameters)
    {
        Parameters = parameters;
    }

    public static FSRS NewFSRS(Parameters p) => new FSRS(p);

        
    public RecordLog Repeat(Card card, DateTime now)
    {
        var scheduler = SchedulerFactory.SchedulerFor(Parameters, card, now);
        return scheduler.Preview();
    }

    //call after user reviews a card
    public SchedulingInfo Next(Card card, DateTime now, Rating grade)
    {
        var scheduler = SchedulerFactory.SchedulerFor(Parameters, card, now);
        return scheduler.Review(grade);
    }

    public double GetRetrievability(Card card, DateTime now)
    {
        if (card.State == State.New) return 0.0;
        double elapsedDays = (now - card.LastReview).TotalHours / 24.0;
        return Parameters.ForgettingCurve(elapsedDays, card.Stability);
    }
}