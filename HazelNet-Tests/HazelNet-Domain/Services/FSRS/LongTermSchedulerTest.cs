using HazelNet_Domain.Models;
using HazelNet_Domain.Services.FSRS;
using Xunit;
using FluentAssertions;

namespace HazelNet_Tests.HazelNet_Domain.Services.FSRS;

public class LongTermSchedulerTest
{
    private readonly DateTime _now = new DateTime(2026, 2, 5, 12, 0, 0, DateTimeKind.Local);
    private readonly Parameters _param = Parameters.DefaultParam();

    private Scheduler CreateScheduler(Card card)
    {
        return Scheduler.NewScheduler(_param, card, _now, s => new LongTermScheduler(s));
    }

    [Fact]
    public void LongTermScheduler_NewState_SwitchesToReview_ForAllRatings()
    {
        // arrange
        var card = new Card() { FrontOfCard = "front", State = State.New };
        var sched = CreateScheduler(card);
        // act
            // Calls NewState() via switch case from Scheduler class' Review() method
        var resultAgain = sched.Review(Rating.Again);
        var resultHard = sched.Review(Rating.Hard);
        var resultGood = sched.Review(Rating.Good);
        var resultEasy = sched.Review(Rating.Easy);
        // assert
        resultAgain.Card.State.Should().Be(State.Review);
        resultHard.Card.State.Should().Be(State.Review);
        resultGood.Card.State.Should().Be(State.Review);
        resultEasy.Card.State.Should().Be(State.Review);
    }

    [Fact]
    public void LongTermScheduler_NewState_IntervalsAreStrictlyIncreasing()
    {
        // arrange
        var card = new Card { FrontOfCard = "front", State = State.New };
        var sched = CreateScheduler(card);
        // act
        var resultAgain = sched.Review(Rating.Again);
        var resultHard = sched.Review(Rating.Hard);
        var resultGood = sched.Review(Rating.Good);
        var resultEasy = sched.Review(Rating.Easy);
        // assert
        resultAgain.Card.ScheduledDays.Should().BeLessThan(resultHard.Card.ScheduledDays);
        resultHard.Card.ScheduledDays.Should().BeLessThan(resultGood.Card.ScheduledDays);
        resultGood.Card.ScheduledDays.Should().BeLessThan(resultEasy.Card.ScheduledDays);
    }

    [Fact]
    public void LongTermScheduler_NewState_AgainIncrementsLapses()
    {
        // arrange
        var card = new Card
        {
            FrontOfCard = "front",
            State = State.Review,
            Stability = 10.0,
            Difficulty = 5.0,
            ElapsedDays = 10,
            Lapses = 0
        };
        var sched = CreateScheduler(card);
        
        // act
        var result = sched.Review(Rating.Again);
        // assert
        result.Card.Lapses.Should().Be(1);
        result.Card.State.Should().Be(State.Review);
    }

    [Fact]
    public void LongTermScheduler_NewState_UpdatesStabilityAndDifficulty()
    {
        // arrange
        var card = new Card
        {
            FrontOfCard = "front",
            State = State.Review,
            Stability = 10.0,
            Difficulty = 5.0,
            ElapsedDays = 10
        };
        var sched = CreateScheduler(card);
        
        // act
        var result = sched.Review(Rating.Good);
        // assert
        result.Card.Stability.Should().NotBe(card.Stability);
        result.Card.Difficulty.Should().NotBe(card.Difficulty);
        result.Card.State.Should().Be(State.Review);
    }

    [Theory]
    [InlineData(Rating.Again)]
    [InlineData(Rating.Hard)]
    [InlineData(Rating.Good)]
    [InlineData(Rating.Easy)]
    public void LongTermScheduler_LearningState_DelegatesToReviewState(Rating rating)
    {
        // arrange
        var card = new Card
        {
            FrontOfCard = "front",
            Stability = 5.0,
            Difficulty = 5.0,
            ElapsedDays = 1
        };
        var sched = CreateScheduler(card);

        // act
        var result = sched.Review(rating);
        // assert
        result.Card.State.Should().Be(State.Review);
        result.Card.ScheduledDays.Should().BeGreaterThan(0);
    }
}