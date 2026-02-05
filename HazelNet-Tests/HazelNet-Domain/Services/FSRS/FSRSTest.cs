using HazelNet_Domain.Models;
using HazelNet_Domain.Services.FSRS;
using Xunit;
using FluentAssertions;

namespace HazelNet_Tests.HazelNet_Domain.Services.FSRS;

public class FSRSTest
{
    private readonly DateTime _now = new DateTime(2026, 2, 5, 12, 0, 0, DateTimeKind.Local);
    private readonly Parameters _param = Parameters.DefaultParam();
    
    [Fact]
    public void FSRS_Repeat_ReturnsRecordLog()
    {
        // arrange
        var fsrs = global::HazelNet_Domain.Services.FSRS.FSRS.NewFSRS(_param);
        var card = new Card { FrontOfCard = "front", State = State.New };
        // act
        var log = fsrs.Repeat(card, _now);
        // assert
        log.Should().NotBeNull();
    }

    [Fact]
    public void FSRS_Next_ReturnsSchedulingInfo()
    {
        // arrange
        var fsrs = global::HazelNet_Domain.Services.FSRS.FSRS.NewFSRS(_param);
        var card = new Card { FrontOfCard = "front", State = State.New };
        // act
        var info = fsrs.Next(card, _now, Rating.Good);
        // assert
        info.Should().NotBeNull();
        info.Card.Should().NotBeNull();
        info.ReviewLog.Should().NotBeNull();
        info.ReviewLog.Rating.Should().Be(Rating.Good);
    }

    [Fact]
    public void FSRS_GetRetrievability_NewCardReturnsZero()
    {
        // arrange
        var fsrs = global::HazelNet_Domain.Services.FSRS.FSRS.NewFSRS(_param);
        var card = new Card {FrontOfCard = "front", State = State.New};
        // act
        var result = fsrs.GetRetrievability(card, _now);
        // assert
        result.Should().Be(0.0);
    }

    [Fact]
    public void FSRS_GetRetrievability_ReviewCardCalculatesCorrectly()
    {
        // arrange
        var fsrs = global::HazelNet_Domain.Services.FSRS.FSRS.NewFSRS(_param);
        var card = new Card
        {
            FrontOfCard = "front",
            State = State.Review,
            Stability = 5.0,
            LastReview = _now.AddDays(-2)
        };
        
        // act
        var result = fsrs.GetRetrievability(card, _now);
        // assert
            // Expected = ForgettingCurve(elapsed=2, stability=5)
            // ForgettingCurve usually: (1 + factor * elapsed / stability) ^ decay
            // or similar. We verify it's a valid probability between 0 and 1 (exclusive usually for non-zero params).
        result.Should().BeGreaterThan(0.0);
        result.Should().BeLessThan(1.0);
        
            // Verify it matches manual calculation call to Parameters if we want strictness,
            // but general expected range is good for integration style unit test of FSRS facade.
        var expected = _param.ForgettingCurve(2.0, 5.0);
        result.Should().Be(expected);
    }
}