using HazelNet_Domain.Models;
using HazelNet_Domain.Services.FSRS;
using Xunit;
using FluentAssertions;

namespace HazelNet_Tests.HazelNet_Domain.Services.FSRS;

public class BasicSchedulerTest
{
    // created a frozen datetime for consistent assertions
    private readonly DateTime _now = new DateTime(2026, 2, 3, 12, 0, 0, DateTimeKind.Local);
    private readonly Parameters _param = Parameters.DefaultParam();
    
    // helper method for invoking NewScheduler()
    private Scheduler CreateScheduler(Card card)
    {
        return Scheduler.NewScheduler(_param, card, _now, s=> new BasicScheduler(s));
    }

    [Fact]
    public void BasicScheduler_NewState_AgainSchedulesForOneMinute()
    {
        // arrange
        var card = new Card() { FrontOfCard = "front", State = State.New };
        var sched = CreateScheduler(card);
        // act
        var result = sched.Review(Rating.Again);
        // assert
        result.Card.State.Should().Be(State.Learning);
        result.Card.ScheduledDays.Should().Be(0);
        result.Card.Due.Should().Be(_now.AddMinutes(1));
    }

    [Fact]
    public void BasicScheduler_NewState_HardSchedulesForFiveMinutes()
    {
        // arrange
        var card = new Card() { FrontOfCard = "front", State = State.New };
        var sched = CreateScheduler(card);
        // act
        var result = sched.Review(Rating.Hard);
        // assert
        result.Card.State.Should().Be(State.Learning);
        result.Card.ScheduledDays.Should().Be(0);
        result.Card.Due.Should().Be(_now.AddMinutes(5));
    }

    [Fact]
    public void BasicScheduler_NewState_GoodSchedulesForTenMinutes()
    {
        // arrange
        var card = new Card() { FrontOfCard = "front", State = State.New };
        var sched = CreateScheduler(card);
        // act
        var result = sched.Review(Rating.Good);
        // assert
        result.Card.State.Should().Be(State.Learning);
        result.Card.ScheduledDays.Should().Be(0);
        result.Card.Due.Should().Be(_now.AddMinutes(10));
    }

    [Fact]
    public void BasicScheduler_NewState_EasySchedulesForDays_AndSwitchesToReview()
    {
        // arrange
        var card = new Card() { FrontOfCard = "front", State = State.New };
        var sched = CreateScheduler(card);
        // act
        var result = sched.Review(Rating.Easy);
        // assert
        result.Card.State.Should().Be(State.Review);
        result.Card.ScheduledDays.Should().BeGreaterThan(0);
        result.Card.Due.Should().BeAfter(_now);
    }

    [Fact]
    public void BasicScheduler_LearningState_AgainResetsShortTerm_AndPreservesState()
    {
        // arrange
        var card = new Card()
        {
            FrontOfCard = "front", 
            State = State.Learning,
            Stability = 2.0,
            Difficulty = 5.0
        };

        var sched = CreateScheduler(card);
        // act
            // LearningState() is invoked via Review() in its switch case 
        var result = sched.Review(Rating.Again);
        // assert
        result.Card.State.Should().Be(State.Learning);
        result.Card.Due.Should().Be(_now.AddMinutes(5));
        result.Card.ScheduledDays.Should().Be(0);
    }

    [Fact]
    public void BasicScheduler_LearningState_HardSchedulesForTenMinutes()
    {
        // arrange
        var card = new Card { FrontOfCard = "front", State = State.Learning };
        var sched = CreateScheduler(card);
        // act
        var result = sched.Review(Rating.Hard);
        // assert
        result.Card.State.Should().Be(State.Learning);
        result.Card.ScheduledDays.Should().Be(0);
        result.Card.Due.Should().Be(_now.AddMinutes(10));
    }

    [Fact]
    public void BasicScheduler_LearningState_GoodIncreasesInterval_AndSwitchesToReview()
    {
        // arrange
        var card = new Card
        {
            FrontOfCard = "front",
            State = State.Learning,
            Stability = 5.0,
            ElapsedDays = 1
        };
         var sched = CreateScheduler(card);
         // act
         var result = sched.Review(Rating.Good);
         // assert
         result.Card.State.Should().Be(State.Review);
         result.Card.ScheduledDays.Should().BeGreaterThan(0);
         result.Card.Due.Should().Be(_now.AddDays(result.Card.ScheduledDays));
    }

    [Fact]
    public void BasicScheduler_LearningState_EasyIncreasesIntervalLarger_AndSwitchesToReview()
    {
        // arrange
        var card = new Card
        {
            FrontOfCard = "front",
            State = State.Learning,
            Stability = 5.0,
            ElapsedDays = 1
        };
        var sched = CreateScheduler(card);
        // act
        var resultGood = sched.Review(Rating.Good);
        
            // create another Scheduler object for a card with a rating of Good for fair comparison
        sched = CreateScheduler(card);
        var resultEasy = sched.Review(Rating.Easy);
        
        // assert
        resultEasy.Card.State.Should().Be(State.Review);
        resultEasy.Card.ScheduledDays.Should().BeGreaterThan(resultGood.Card.ScheduledDays);
    }

    [Fact]
    public void BasicScheduler_ReviewState_GoodShouldUpdateStabilityAndDifficulty()
    {
        // arrange
        var card = new Card
        {
            FrontOfCard = "front",
            State = State.Review,
            Stability = 10.0,
            Difficulty = 5.0,
            ElapsedDays = 10,
            LastReview = _now.AddDays(-10)
        };
        var sched = CreateScheduler(card);
        // act
        var result = sched.Review(Rating.Good);
        // assert
        result.Card.State.Should().Be(State.Review);
        result.Card.Stability.Should().NotBe(card.Stability);
        result.Card.Difficulty.Should().NotBe(card.Difficulty);
        result.Card.ScheduledDays.Should().BeGreaterThan(0);
    }

    [Fact]
    public void BasicScheduler_ReviewState_AgainShouldLapseAndResetInterval()
    {
        // arrange
        var card = new Card
        {
            FrontOfCard = "front",
            State = State.Review,
            Stability = 10.0,
            Difficulty = 5.0,
            Lapses = 0
        };
        var sched = CreateScheduler(card);
        // act
        var result = sched.Review(Rating.Again);
        // assert
        result.Card.State.Should().Be(State.Relearning);
        result.Card.Lapses.Should().Be(1);
        result.Card.Due.Should().Be(_now.AddMinutes(5));
        result.Card.ScheduledDays.Should().Be(0);
        result.Card.Difficulty.Should().NotBe(card.Difficulty);
    }
}