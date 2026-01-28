using HazelNet_Domain.Models;

namespace HazelNet_Domain.Services.FSRS;

public class LongTermScheduler : IImplScheduler
{
    private readonly Scheduler s;
    private Parameters p => s.parameters;

    public LongTermScheduler(Scheduler scheduler)
    {
        s = scheduler;
    }

    public SchedulingInfo NewState(Rating grade)
    {
        if (s.next.TryGetValue(grade, out var exist)) return exist;

        s.current.ScheduledDays = 0;
        s.current.ElapsedDays = 0;

        var nextAgain = s.current.Clone();
        var nextHard = s.current.Clone();
        var nextGood = s.current.Clone();
        var nextEasy = s.current.Clone();

        InitDs(nextAgain, nextHard, nextGood, nextEasy);

        NextInterval(nextAgain, nextHard, nextGood, nextEasy, 0);
        NextState(nextAgain, nextHard, nextGood, nextEasy);
        UpdateNext(nextAgain, nextHard, nextGood, nextEasy);

        return s.next[grade];
    }

    public SchedulingInfo LearningState(Rating grade)
    {
        return ReviewState(grade);
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

        UpdateNext(nextAgain, nextHard, nextGood, nextEasy);
        return s.next[grade];
    }

    private void InitDs(Card a, Card h, Card g, Card e)
    {
        a.Difficulty = p.InitDifficulty(Rating.Again);
        a.Stability = p.InitStability(Rating.Again);

        h.Difficulty = p.InitDifficulty(Rating.Hard);
        h.Stability = p.InitStability(Rating.Hard);

        g.Difficulty = p.InitDifficulty(Rating.Good);
        g.Stability = p.InitStability(Rating.Good);

        e.Difficulty = p.InitDifficulty(Rating.Easy);
        e.Stability = p.InitStability(Rating.Easy);
    }

    private void NextDs(Card a, Card h, Card g, Card e, double difficulty, double stability, double retrievability)
    {
        a.Difficulty = p.NextDifficulty(difficulty, Rating.Again);
        a.Stability = Math.Min(stability, p.NextForgetStability(difficulty, stability, retrievability));

        h.Difficulty = p.NextDifficulty(difficulty, Rating.Hard);
        h.Stability = p.NextRecallStability(difficulty, stability, retrievability, Rating.Hard);

        g.Difficulty = p.NextDifficulty(difficulty, Rating.Good);
        g.Stability = p.NextRecallStability(difficulty, stability, retrievability, Rating.Good);

        e.Difficulty = p.NextDifficulty(difficulty, Rating.Easy);
        e.Stability = p.NextRecallStability(difficulty, stability, retrievability, Rating.Easy);
    }

    private void NextInterval(Card a, Card h, Card g, Card e, double elapsedDays)
    {
        double againInterval = p.NextInterval(a.Stability, elapsedDays);
        double hardInterval = p.NextInterval(h.Stability, elapsedDays);
        double goodInterval = p.NextInterval(g.Stability, elapsedDays);
        double easyInterval = p.NextInterval(e.Stability, elapsedDays);

        againInterval = Math.Min(againInterval, hardInterval);
        hardInterval = Math.Max(hardInterval, againInterval + 1);
        goodInterval = Math.Max(goodInterval, hardInterval + 1);
        easyInterval = Math.Max(easyInterval, goodInterval + 1);

        a.ScheduledDays = (ulong)againInterval;
        a.Due = s.now.AddDays(againInterval);

        h.ScheduledDays = (ulong)hardInterval;
        h.Due = s.now.AddDays(hardInterval);

        g.ScheduledDays = (ulong)goodInterval;
        g.Due = s.now.AddDays(goodInterval);

        e.ScheduledDays = (ulong)easyInterval;
        e.Due = s.now.AddDays(easyInterval);
    }

    private void NextState(Card a, Card h, Card g, Card e)
    {
        a.State = State.Review;
        h.State = State.Review;
        g.State = State.Review;
        e.State = State.Review;
    }

    private void UpdateNext(Card a, Card h, Card g, Card e)
    {
        s.next[Rating.Again] = new SchedulingInfo { Card = a, ReviewLog = s.BuildLog(Rating.Again) };
        s.next[Rating.Hard] = new SchedulingInfo { Card = h, ReviewLog = s.BuildLog(Rating.Hard) };
        s.next[Rating.Good] = new SchedulingInfo { Card = g, ReviewLog = s.BuildLog(Rating.Good) };
        s.next[Rating.Easy] = new SchedulingInfo { Card = e, ReviewLog = s.BuildLog(Rating.Easy) };
    }
}