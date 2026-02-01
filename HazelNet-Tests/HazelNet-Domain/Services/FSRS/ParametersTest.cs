using System.Reflection;
using HazelNet_Domain.Models;
using HazelNet_Domain.Services.FSRS;

namespace HazelNet_Tests.HazelNet_Domain.Services.FSRS;
using Xunit;
using FluentAssertions;


public class ParametersTest
{
    // SUT
    private readonly Parameters _paramObj = new Parameters(); 
    
    
    [Fact]
    public void Parameters_DefaultParam_CorrectlySetsDefaultParams()
    {
        // arrange 
        // act
        var paramObj = Parameters.DefaultParam();
        // assert
        paramObj.RequestRetention.Should().BePositive();
        paramObj.RequestRetention.Should().NotBeNaN();
        paramObj.RequestRetention.Should().BeOfType(typeof(double));
        paramObj.MaximumInterval.Should().BePositive();
        paramObj.MaximumInterval.Should().BeOfType(typeof(double));
        paramObj.MaximumInterval.Should().BeInRange(1, 365);
    }

    
    [Theory]
    [InlineData(28, 31.6058)]
    [InlineData(56, 0.9205)]
    [InlineData(162, 1.5181)]
    public void Parameters_ForgettingCurve_ReturnsProbabilityOfTypeDouble(double days, double stability)
    {
        // arrange
        // act
        var result = _paramObj.ForgettingCurve(days, stability);
        // assert
        result.Should().NotBeNaN();
        result.Should().BeOfType(typeof(double));
        result.Should().BePositive();
        result.Should().BeInRange(0.01, 1);
    }
    
    
    [Theory]
    [InlineData(0.0027)]
    [InlineData(19.033)]
    [InlineData(6.42)]
    public void Parameters_ConstrainDifficulty_ClampsDifficultyAndReturnsDouble(double input)
    {
        // arrange
        
            // This is a reflection wrapper used to access the static method ConstrainDifficulty() for testing.
            // This allows the testing of methods without changing their access modifiers but will fail if the 
            // original method name is changed
        
        var ConstrainDifficulty = typeof(Parameters).GetMethod("ConstrainDifficulty",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        // act
        var result = (double)ConstrainDifficulty.Invoke(null, new object[] { input });
        // assert
        result.Should().NotBeNaN();
        result.Should().BeInRange(1, 10);
    }

    
    // just a collection of test inlineData for the next 2 methods to avoid repetition
    public static IEnumerable<object[]> RatingTestData =>
        new List<object[]>
        {
            new object[] { Rating.Easy },
            new object[] { Rating.Good },
            new object[] { Rating.Hard },
            new object[] { Rating.Again }
        };
    
    
    
    [Theory]
    [MemberData(nameof(RatingTestData))]
    public void Parameters_InitStability_ReturnsDouble(Rating rating)
    {
        // arrange
        // act
        var result = _paramObj.InitStability(rating);
        // assert
        result.Should().BeOfType(typeof(double));
        result.Should().BePositive();
        result.Should().BeGreaterThanOrEqualTo(0.0001);
    }
    
    
    [Theory]
    [MemberData(nameof(RatingTestData))]
    public void Parameters_InitDifficulty_ReturnsDoubleFromRatings(Rating diff)
    {
        // arrange
        // act
        var result = _paramObj.InitDifficulty(diff);
        // assert
        result.Should().NotBeNaN();
        result.Should().BeOfType(typeof(double));
        result.Should().BeInRange(1, 10);
    }
    
    
    [Theory]
    [InlineData(2.83, 8.801)]
    [InlineData(-33.21, 6.023)]
    [InlineData(310.67, 9.097)]
    public void Parameters_LinearDamping_ReducesToRangeZeroToOne(double newDelta, double oldDelta)
    {
        // arrange
        
            // another reflection wrapper for LinearDamping
        var LinearDamping = typeof(Parameters).GetMethod("LinearDamping",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        // act
        var result = (double)LinearDamping.Invoke(null, new object[] {newDelta, oldDelta});
        // assert
        result.Should().NotBeNaN();
        result.Should().BeInRange(0, 1);
    }

    [Theory]
    [InlineData(0.01, 93)]
    [InlineData(12.497, 127)]
    [InlineData(2.067, 354)]
    public void Parameters_NextInterval_ReturnsNumberOfDaysInYear(double s, double elapsedDays)
    {
        // arrange
        // act
        var result = _paramObj.NextInterval(s, elapsedDays);
        // assert
        result.Should().NotBeNaN();
        result.Should().BeOfType(typeof(double));
        result.Should().BeInRange(1, 365);
    }
    
    [Theory]
    [InlineData(44, 108, 365)]
    [InlineData(89, 270, 365)]
    public void Parameters_GetFuzzRange_ReturnsIntTuple(double interval, double elapsedDays, double maxInterval)
    {
        // arrange
        var GetFuzzRange = typeof(Parameters).GetMethod("GetFuzzRange",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        // act
        var (x, y) = ((int minIvlFloat, int maxIvlFloat))GetFuzzRange.Invoke(_paramObj, new object[] {interval, elapsedDays, maxInterval} );
        // assert
        x.Should().BeGreaterThanOrEqualTo(2);
        y.Should().BeLessThanOrEqualTo(365);
    }

    
    // another collection of test data
    public static IEnumerable<object[]> DifficultyStabilityData =>
        new List<object[]>
        {
            new object[] { 3.002, Rating.Again },
            new object[] { 1.047, Rating.Easy },
            new object[] { 7.4908, Rating.Good },
            new object[] { 9.992, Rating.Hard }
        };
    
    
    
    [Theory]
    [MemberData(nameof(DifficultyStabilityData))]
    public void Parameters_NextDifficulty_ReturnsDoubleInRange(double d, Rating r)
    {
        // arrange
        // act
        var result = _paramObj.NextDifficulty(d, r);
        // assert
        result.Should().NotBeNaN();
        result.Should().BeOfType(typeof(double));
        result.Should().BeInRange(1, 10);
    }

    [Theory]
    [MemberData(nameof(DifficultyStabilityData))]
    public void Parameters_ShortTermStability_ReturnsDoubleUpToInfinity(double s, Rating r)
    {
     // arrange
     // act
     var result = _paramObj.ShortTermStability(s, r);
     // assert
     result.Should().NotBeNaN();
     result.Should().BeGreaterThanOrEqualTo(0.01);
    }

    [Theory]
    [InlineData(3.002, 7.23)]
    [InlineData(6.6692, 1.003)]
    [InlineData(2.718, 3.14)]
    public void Parameters_MeanReversion_ReturnsDoubleInRange(double init, double curr)
    {
        // arrange
        // act
        var result = _paramObj.MeanReversion(init, curr);
        // assert
        result.Should().NotBeNaN();
        result.Should().BeInRange(1, 10);
    }

    [Theory]
    [InlineData(1.662, 11.4597, 0.887, Rating.Again)]
    [InlineData(3.9819, 21.8278, 0.178, Rating.Hard)]
    [InlineData(7.5489, 43.9088, 0.2257, Rating.Easy)]
    public void Parameters_NextRecallStability_ReturnsDoubleUpToInfinity(double d, double s, double r, Rating rating)
    {
        // arrange 
        // act
        var result = _paramObj.NextRecallStability(d, s, r, rating);
        // assert
        result.Should().NotBeNaN();
        result.Should().BeGreaterThanOrEqualTo(0.01);
    }

    [Theory]
    [InlineData(1.662, 11.4597, 0.887)]
    [InlineData(3.9819, 21.8278, 0.178)]
    [InlineData(7.5489, 43.9088, 0.2257)]
    public void Parameters_NextForgetStability_ReturnsDoubleUpToInfinity(double d, double s, double r)
    {
        // arrange
        // act
        var result = _paramObj.NextForgetStability(d, s, r);
        // assert
        result.Should().NotBeNaN();
        result.Should().BeGreaterThanOrEqualTo(0.01);
    }
}