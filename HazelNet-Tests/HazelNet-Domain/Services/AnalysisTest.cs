using HazelNet_Domain.Services.FSRS;
using HazelNet_Domain.Models;

namespace HazelNet_Tests.HazelNet_Domain.Services;
using Xunit;
using FluentAssertions;

public class AnalysisTest
{
    // SUT
    private readonly Analysis _analysis = new();

    [Theory]
    [InlineData(27, 8, 0.7037037037)]
    [InlineData(34, 5, 0.85294117647)]
    [InlineData(51, 8, 0.8431372549)]
    public void Analysis_RecallAccuracy_ReturnsDouble(ulong rep, ulong lapse, double expected)
    {
        // arrange
        Card card = new()
        {
            FrontOfCard = "cardfront",
            Reps = rep,
            Lapses = lapse
        };
        // act
        var result = _analysis.RecallAccuracy(card);
        // assert
        result.Should().NotBeNaN();
        result.Should().BeApproximately(expected, 0.0001);
    }

    [Fact]
    public void Analysis_AvgDifficulty_ReturnsRatingEnum() {
        // arrange
        var revHist = new ReviewHistory(1)
        {
            ReviewLogs = {
                new ReviewLog() { Rating = Rating.Again }, // 1
                new ReviewLog() { Rating = Rating.Easy },// 4
                new ReviewLog() { Rating = Rating.Hard },// 2
                new ReviewLog() { Rating = Rating.Hard },// 2
                new ReviewLog() { Rating = Rating.Good }// 3
            }
        };
        // act
        var result = _analysis.AvgDifficulty(revHist);
        // assert
        result.Should().BeOneOf(Rating.Hard, Rating.Good, Rating.Easy);
    }

}
