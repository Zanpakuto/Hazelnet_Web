using HazelNet_Domain.Models;

namespace HazelNet_Domain.Services.FSRS;

public class BasicScheduler : IImplScheduler
{
    private readonly Scheduler s;
    private Parameters p => s.parameters;

    public BasicScheduler(Scheduler scheduler)
    {
        s = scheduler;
    }
    
    //setup card upon creation
    public SchedulingInfo NewState(Rating grade)
    {
        if (s.next.TryGetValue(grade, out var exist)) return exist;

        var next = s.current.Clone();
        next.Difficulty = p.InitDifficulty(grade);
        next.Stability = p.InitStability(grade);

        //ensures card is scheduled on the same day except if initial grade is easy
        switch (grade)
        {
            case Rating.Again:
                next.ScheduledDays = 0;
                next.Due = s.now.AddMinutes(1);
                next.State = State.Learning;
                break;
            case Rating.Hard:
                next.ScheduledDays = 0;
                next.Due = s.now.AddMinutes(5);
                next.State = State.Learning;
                break;
            case Rating.Good:
                next.ScheduledDays = 0;
                next.Due = s.now.AddMinutes(10);
                next.State = State.Learning;
                break;
            case Rating.Easy:
                var easyInterval = p.NextInterval(next.Stability, next.ElapsedDays);
                next.ScheduledDays = (ulong)easyInterval;
                next.Due = s.now.AddDays(easyInterval);
                next.State = State.Review;
                break;
        }

        var item = new SchedulingInfo { Card = next, ReviewLog = s.BuildLog(grade) };
        s.next[grade] = item;
        return item;
    }

    public SchedulingInfo LearningState(Rating grade)
    {
        if (s.next.TryGetValue(grade, out var exist)) return exist;

        var next = s.current.Clone();
        double interval = s.current.ElapsedDays;
        next.Difficulty = p.NextDifficulty(s.last.Difficulty, grade);
        next.Stability = p.ShortTermStability(s.last.Stability, grade);
     

        switch (grade)
        {
            case Rating.Again:
                next.ScheduledDays = 0;
                next.Due = s.now.AddMinutes(5);
                next.State = s.last.State;
                break;
            case Rating.Hard:
                next.ScheduledDays = 0;
                next.Due = s.now.AddMinutes(10);
                next.State = s.last.State;
                break;
            case Rating.Good:
                var goodInterval = p.NextInterval(next.Stability, interval);
                next.ScheduledDays = (ulong)goodInterval;
                next.Due = s.now.AddDays(goodInterval);
                next.State = State.Review;
                break;
            case Rating.Easy:
                var goodStability = p.ShortTermStability(s.last.Stability, Rating.Good);
                var gi = p.NextInterval(goodStability, interval);
                var ei = Math.Max(p.NextInterval(next.Stability, interval), gi + 1);
                next.ScheduledDays = (ulong)ei;
                next.Due = s.now.AddDays(ei);
                next.State = State.Review;
                break;
        }

        var item = new SchedulingInfo { Card = next, ReviewLog = s.BuildLog(grade) };
        s.next[grade] = item;
        return item;
    }

    public SchedulingInfo ReviewState(Rating grade)
    {
        if (s.next.TryGetValue(grade, out var exist)) return exist;

        double interval = s.current.ElapsedDays;
        double difficulty = s.last.Difficulty;
        double stability = s.last.Stability;
        double retrievability = p.ForgettingCurve(interval, stability);

        var nextAgain = s.current.Clone();
        var nextHard = s.current.Clone();
        var nextGood = s.current.Clone();
        var nextEasy = s.current.Clone();

        NextDs(nextAgain, nextHard, nextGood, nextEasy, difficulty, stability, retrievability);
        NextInterval(nextAgain, nextHard, nextGood, nextEasy, interval);
        NextState(nextAgain, nextHard, nextGood, nextEasy);
        nextAgain.Lapses++;

        var ia = new SchedulingInfo { Card = nextAgain, ReviewLog = s.BuildLog(Rating.Again) };
        var ih = new SchedulingInfo { Card = nextHard, ReviewLog = s.BuildLog(Rating.Hard) };
        var ig = new SchedulingInfo { Card = nextGood, ReviewLog = s.BuildLog(Rating.Good) };
        var ie = new SchedulingInfo { Card = nextEasy, ReviewLog = s.BuildLog(Rating.Easy) };

        s.next[Rating.Again] = ia;
        s.next[Rating.Hard] = ih;
        s.next[Rating.Good] = ig;
        s.next[Rating.Easy] = ie;

        return s.next[grade];
    }

    private void NextDs(Card a, Card h, Card g, Card e, double difficulty, double stability, double retrievability)
    {
        a.Difficulty = p.NextDifficulty(difficulty, Rating.Again);
        double nextSMin = stability / Math.Exp(p.W[17] * p.W[18]);
        a.Stability = Math.Min(nextSMin, p.NextForgetStability(difficulty, stability, retrievability));

        h.Difficulty = p.NextDifficulty(difficulty, Rating.Hard);
        h.Stability = p.NextRecallStability(difficulty, stability, retrievability, Rating.Hard);

        g.Difficulty = p.NextDifficulty(difficulty, Rating.Good);
        g.Stability = p.NextRecallStability(difficulty, stability, retrievability, Rating.Good);

        e.Difficulty = p.NextDifficulty(difficulty, Rating.Easy);
        e.Stability = p.NextRecallStability(difficulty, stability, retrievability, Rating.Easy);
        
    }

    private void NextInterval(Card a, Card h, Card g, Card e, double elapsedDays)
    {
        double hardInterval = p.NextInterval(h.Stability, elapsedDays);
        double goodInterval = p.NextInterval(g.Stability, elapsedDays);
        hardInterval = Math.Min(hardInterval, goodInterval);
        goodInterval = Math.Max(goodInterval, hardInterval + 1);
        double easyInterval = Math.Max(p.NextInterval(e.Stability, elapsedDays), goodInterval + 1);

        a.ScheduledDays = 0;
        a.Due = s.now.AddMinutes(5);

        h.ScheduledDays = (ulong)hardInterval;
        h.Due = s.now.AddDays(hardInterval);

        g.ScheduledDays = (ulong)goodInterval;
        g.Due = s.now.AddDays(goodInterval);

        e.ScheduledDays = (ulong)easyInterval;
        e.Due = s.now.AddDays(easyInterval);
    }

    private void NextState(Card a, Card h, Card g, Card e)
    {
        a.State = State.Relearning;
        h.State = State.Review;
        g.State = State.Review;
        e.State = State.Review;
    }
}