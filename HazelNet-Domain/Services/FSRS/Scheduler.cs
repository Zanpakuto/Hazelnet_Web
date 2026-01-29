using HazelNet_Domain.Models;

namespace HazelNet_Domain.Services.FSRS;

public class Scheduler
{
    internal Parameters parameters;
    internal Card last;
    internal Card current;
    internal DateTime now;
    internal RecordLog next;
    internal IImplScheduler impl;

    public Scheduler(Parameters p)
    {
        parameters = p;
        next = new RecordLog();
    }

    //previews the results of reviewing a card with all possible ratings
    public RecordLog Preview()
    {
        var log = new RecordLog();
        log[Rating.Again] = Review(Rating.Again);
        log[Rating.Hard] = Review(Rating.Hard);
        log[Rating.Good] = Review(Rating.Good);
        log[Rating.Easy] = Review(Rating.Easy);
        return log;
    }

    //reviews the card with the given rating and returns the scheduling info
    public SchedulingInfo Review(Rating grade)
    {
        var state = last.State;
        SchedulingInfo item = null;
        switch (state)
        {
            case State.New:
                item = impl.NewState(grade);
                break;
            case State.Learning:   
            case State.Relearning:
                item = impl.LearningState(grade);
                break;
            case State.Review:
                item = impl.ReviewState(grade);
                break;
        }
        return item;
    }

    //initializes the seed used for fuzzing
    internal void InitSeed()
    {
        long unix = ((DateTimeOffset)now).ToUnixTimeSeconds();
        double mul = current.Difficulty * current.Stability;
        parameters.seed = $"{unix}_{current.Reps}_{mul}";
    }

    //builds a review log for the current review
    internal ReviewLog BuildLog(Rating rating)
    {
        return new ReviewLog
        {
            Rating = rating,
            State = current.State,
            ElapsedDays = current.ElapsedDays,
            ScheduledDays = current.ScheduledDays,
            Review = now
        };
    }

    //called whenever a card is reviewed to schedule its next review
    public static Scheduler NewScheduler(Parameters p, Card card, DateTime now, Func<Scheduler, IImplScheduler> createImpl)
    {
        var s = new Scheduler(p)
        {
            last = card.Clone(),
            current = card.Clone(),
            now = now,
            parameters = p,
            next = new RecordLog()
        };

        double interval = 0;
        if (s.current.State != State.New && s.current.LastReview != DateTime.MinValue)
        {
            interval = Math.Floor((s.now - s.current.LastReview).TotalHours / 24.0);
        }
        s.current.LastReview = now;
        s.current.ElapsedDays = (ulong)interval;
        s.current.Reps++;
        s.InitSeed();

        s.impl = createImpl(s);
        return s;
    }
}